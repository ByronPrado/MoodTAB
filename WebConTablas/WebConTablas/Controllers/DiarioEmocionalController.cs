using Microsoft.AspNetCore.Mvc;
using WebConTablas.Models;
using WebConTablas.Services;
using WebConTablas.ML; 
using System; 
using System.Linq; 
using System.Threading.Tasks;
using WebConTablas.Controllers; 
using WebConTablas.Common; // Asegúrate de que este using esté presente
using System.Collections.Generic; 

[ApiController]
[Route("api/[controller]")]
public class DiarioEmocionalController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly EmotionalPredictionService _predictionService;
    private readonly ChatService _chatService; 

    public DiarioEmocionalController(AppDbContext context, EmotionalPredictionService predictionService, ChatService chatService)
    {
        _context = context;
        _predictionService = predictionService;
        _chatService = chatService; 
    }

    [HttpPost]
    public async Task<IActionResult> PostDiario(DiarioEmocional diario)
    {
        // 1. Conversión de fecha a UTC (sin cambios)
        if (diario.Fecha.Kind == DateTimeKind.Unspecified)
        {
            diario.Fecha = DateTime.SpecifyKind(diario.Fecha, DateTimeKind.Local).ToUniversalTime();
        }
        else if (diario.Fecha.Kind == DateTimeKind.Local)
        {
            diario.Fecha = diario.Fecha.ToUniversalTime();
        }

        // 2. Preparar los datos de entrada para el modelo ML y el análisis de IA.
        // -----------------------------------------------------------
        string[] zonasTexto = new string[3];
        var partes = diario.Descripcion?
            .Split(new char[] { '/' }, 3, StringSplitOptions.RemoveEmptyEntries)
            .Select(p => p.Trim())
            .ToArray() ?? Array.Empty<string>();

        zonasTexto[0] = partes.Length > 0 ? partes[0] : string.Empty;
        zonasTexto[1] = partes.Length > 1 ? partes[1] : string.Empty;
        zonasTexto[2] = partes.Length > 2 ? partes[2] : string.Empty;
        
        var zonasConContenido = zonasTexto.Where(z => !string.IsNullOrWhiteSpace(z)).ToList();
        int zonaCount = zonasConContenido.Count;

        // **PASO CLAVE: Llamada DUAL a la IA para Errores y Coherencia Semántica**
        if (zonaCount == 0)
        {
            diario.Coherencia = 1.0f; 
            diario.Errores_gramaticales = 0.0f; 
        }
        else
        {
            // 2.1. Crear tareas para Coherencia y Errores (ambos son llamadas a la IA)
            var errorTasks = zonasConContenido
                .Select(zona => _chatService.GetGrammarErrorCountAsync(zona))
                .ToList();
            
            var coherenceTasks = zonasConContenido
                .Select(zona => _chatService.GetSemanticCoherenceAsync(zona))
                .ToList();

            // 2.2. Esperar que TODAS las llamadas de IA terminen de forma asíncrona y paralela
            var erroresPorZona = await Task.WhenAll(errorTasks);
            var coherenciaPorZona = await Task.WhenAll(coherenceTasks);
            
            // 2.3. Cálculo de Promedios
            float totalErroresGramaticales = erroresPorZona.Sum();
            float totalCoherencia = coherenciaPorZona.Sum();

            float promedioCoherencia = totalCoherencia / zonaCount; 
            float promedioErrores = totalErroresGramaticales / zonaCount; 
            
            // 🎯 APLICACIÓN DE REDONDEO Y LÍMITE DE DECIMALES 🎯
            
            // Redondear Errores a número entero (0 decimales)
            diario.Errores_gramaticales = (float)Math.Round(promedioErrores, 0); 

            // Limitar Coherencia a un máximo de 1 decimal
            diario.Coherencia = (float)Math.Round(promedioCoherencia, 1); 
        }
        // -----------------------------------------------------------

        // **C. Creación del objeto de datos ML**
        var mlData = new EmotionalStateData
        {
            HR_RitmoCardiaco = diario.HR_RitmoCardiaco.GetValueOrDefault(0f),
            HRV_VariabilidadFrecuencia = diario.HRV_VariabilidadFrecuencia.GetValueOrDefault(0f), 
            PasosDiarios = diario.Pasos.GetValueOrDefault(0),
            
            Hora_dormida = diario.Hora_dormida.GetValueOrDefault(7.0f), 
            Horas_celular = diario.Horas_celular.GetValueOrDefault(0),
            Horas_redes = diario.Horas_redes.GetValueOrDefault(0),

            Emociones = diario.Emociones ?? "5,5,5,5,5",
            
            Zona1_Actividades = zonasTexto[0],
            Zona2_Personas = zonasTexto[1],
            Zona3_Pensamientos = zonasTexto[2],
            
            Coherencia = diario.Coherencia.Value,
            Errores_gramaticales = diario.Errores_gramaticales.Value,
            EstadoEmocional = null 
        };

        // 3. Usar el servicio de ML para obtener el estado emocional
        diario.Estado = _predictionService.PredictState(mlData);

        // 4. Guardar el diario en la base de datos (con los promedios calculados)
        _context.DiariosEmocionales.Add(diario);
        await _context.SaveChangesAsync(); 

        // 5. Retornar el resultado
        return Ok(new 
        { 
            ID_Diario = diario.ID_Diario, 
            Estado = diario.Estado, 
            CoherenciaPromedio = diario.Coherencia,
            ErroresPromedio = diario.Errores_gramaticales,
            PredictionSource = "ML.NET (Integrado con Análisis NLP de IA Dual)" 
        });
    }
}