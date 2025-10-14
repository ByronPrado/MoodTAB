// /ML/EmotionalStateModel.cs
using Microsoft.ML.Data;

namespace WebConTablas.ML
{
public class EmotionalStateData
{
    [LoadColumn(0)] public float HR_RitmoCardiaco { get; set; }
    [LoadColumn(1)] public float HRV_VariabilidadFrecuencia { get; set; }
    [LoadColumn(2)] public float PasosDiarios { get; set; }
    [LoadColumn(3)] public float Hora_dormida { get; set; }
    [LoadColumn(4)] public float Horas_celular { get; set; } // Solo una vez
    [LoadColumn(5)] public float Horas_redes { get; set; }
    [LoadColumn(6)] public string Emociones { get; set; }
    [LoadColumn(7)] public string Zona1_Actividades { get; set; } 
    [LoadColumn(8)] public string Zona2_Personas { get; set; }   
    [LoadColumn(9)] public string Zona3_Pensamientos { get; set; } 
    [LoadColumn(10)] public float Coherencia { get; set; }
    [LoadColumn(11)] public float Errores_gramaticales { get; set; }
    [LoadColumn(12)] public string EstadoEmocional { get; set; }
}

    public class EmotionalStatePrediction
    {
        [ColumnName("Prediction")]
        public string Prediction { get; set; }
    }
}