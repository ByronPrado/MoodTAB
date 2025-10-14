using Microsoft.AspNetCore.Mvc;
using WebConTablas.Models;
using WebConTablas.Services;
using WebConTablas.ML; 
using System; 

[ApiController]
[Route("api/[controller]")]
public class DiarioEmocionalController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly EmotionalPredictionService _predictionService;

    public DiarioEmocionalController(AppDbContext context, EmotionalPredictionService predictionService)
    {
        _context = context;
        _predictionService = predictionService;
    }

    [HttpPost]
    public IActionResult PostDiario(DiarioEmocional diario)
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

        // 2. Preparar los datos de entrada para el modelo ML.
        //-----------------------------------------------------------
        //PENDIENTE: Simulación de Coherencia y Errores_gramaticales (Implementar procesamiento de descripcion)
        //-----------------------------------------------------------
        float coherencia, Errores_gramaticales;

        if (!string.IsNullOrEmpty(diario.Descripcion))
        {
            coherencia = Math.Min(10.0f, (float)diario.Descripcion.Length / 50.0f);
            Errores_gramaticales = 0.1f;
        }
        else
        {
            coherencia = 5.0f;
            Errores_gramaticales = 0.5f;
        }
        //-----------------------------------------------------------

        diario.Coherencia = coherencia;
        diario.Errores_gramaticales = Errores_gramaticales;

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
            
            Coherencia = diario.Coherencia.GetValueOrDefault(5.0f),
            Errores_gramaticales = diario.Errores_gramaticales.GetValueOrDefault(0.5f),
            EstadoEmocional = null 
        };

        // 3. Usar el servicio de ML para obtener el estado emocional
        diario.Estado = _predictionService.PredictState(mlData);

        // 4. Guardar el diario en la base de datos
        _context.DiariosEmocionales.Add(diario);
        _context.SaveChanges();

        // 5. Retornar el resultado
        return Ok(new 
        { 
            ID_Diario = diario.ID_Diario, 
            Estado = diario.Estado, 
            PredictionSource = "ML.NET (Adaptado con Detección NLP Simulada)" 
        });
    }
}