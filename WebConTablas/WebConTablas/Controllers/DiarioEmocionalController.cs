// Controllers/DiarioEmocionalController.cs
using Microsoft.AspNetCore.Mvc;
using WebConTablas.Models;
using WebConTablas.Services;
using WebConTablas.ML; // Para acceder a EmotionalStateData

[ApiController]
[Route("api/[controller]")]
public class DiarioEmocionalController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly EmotionalPredictionService _predictionService;

    // Inyección de dependencias para el contexto de BD y el nuevo servicio de ML
    public DiarioEmocionalController(AppDbContext context, EmotionalPredictionService predictionService)
    {
        _context = context;
        _predictionService = predictionService;
    }

    [HttpPost]
    public IActionResult PostDiario(DiarioEmocional diario)
    {
        // 1. Preparar los datos de entrada para el modelo ML
        if (diario.HR_RitmoCardiaco is null || diario.Pasos is null || diario.Horas_celular is null || diario.Horas_redes is null)
        {
            return BadRequest("Faltan datos de biometría requeridos para la predicción.");
        }

        // Conversión de Horas_dormida (string) a float (HorasSueno)
        if (!float.TryParse(diario.Hora_dormida, out float horasSueno))
        {
            // Asumimos un valor por defecto si no se puede parsear
            horasSueno = 7.0f; 
        }

        // Mapeo del campo Emociones/Descripción a los scores de ML.
        // **NOTA**: Estos valores (7, 5, 0.8f) son de EJEMPLO. En una app real,
        // tendrías que calcularlos a partir de los strings 'Emociones' y 'Descripcion'.
        float deteccionEmocionesScore = 7.0f; // Extraer de diario.Emociones (ej. conteo de palabras clave)
        float contenidoSemanticoScore = 5.0f; // Extraer de diario.Descripcion
        float complejidadLenguajeErrores = 0.8f; // Extraer de diario.Descripcion

        var mlData = new EmotionalStateData
        {
            HR_RitmoCardiaco = diario.HR_RitmoCardiaco.Value,
            HRV_VariabilidadFrecuencia = diario.HRV_VariabilidadFrecuencia.GetValueOrDefault(50f), // Usar default si es nulo
            PasosDiarios = diario.Pasos.Value,
            HorasSueno = horasSueno,
            Temperatura_Desviacion = diario.Temperatura_Desviacion.GetValueOrDefault(0f),
            // Tomamos el MÁXIMO de horas de celular/redes para RedesSociales_Frecuencia (asunción)
            RedesSociales_Frecuencia = System.Math.Max(diario.Horas_celular.Value, diario.Horas_redes.Value), 
            
            // Asignación de Scores de análisis de texto (ejemplos hardcodeados)
            DeteccionEmociones_Subjetivas = deteccionEmocionesScore,
            ContenidoSemantico_KeywordsScore = contenidoSemanticoScore,
            ComplejidadLenguaje_Errores = complejidadLenguajeErrores,
            EstadoEmocional = null // La etiqueta no se usa para la predicción
        };

        // 2. Usar el servicio de ML para obtener el estado emocional
        diario.Estado = _predictionService.PredictState(mlData);

        // 3. Guardar el diario en la base de datos
        _context.DiariosEmocionales.Add(diario);
        _context.SaveChanges();

        // 4. Retornar el resultado
        return Ok(new { diario.ID_Diario, diario.Estado, PredictionSource = "ML.NET" });
    }
}