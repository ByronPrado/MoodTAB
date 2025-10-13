// En una carpeta /ML/EmotionalStateModel.cs
using Microsoft.ML.Data;

namespace WebConTablas.ML
{
    public class EmotionalStateData
    {
        // Mapeo directo a los inputs requeridos por el pipeline
        // NÃ³tese que se usan floats, requeridos por ML.NET
        [LoadColumn(0)] public float HR_RitmoCardiaco { get; set; }
        [LoadColumn(1)] public float HRV_VariabilidadFrecuencia { get; set; }
        [LoadColumn(2)] public float PasosDiarios { get; set; }
        [LoadColumn(3)] public float HorasSueno { get; set; }
        [LoadColumn(4)] public float Temperatura_Desviacion { get; set; }
        [LoadColumn(5)] public float RedesSociales_Frecuencia { get; set; }
        [LoadColumn(6)] public float DeteccionEmociones_Subjetivas { get; set; }
        [LoadColumn(7)] public float ContenidoSemantico_KeywordsScore { get; set; }
        [LoadColumn(8)] public float ComplejidadLenguaje_Errores { get; set; }
        [LoadColumn(9)] public string EstadoEmocional { get; set; } // Etiqueta (Label)
    }

    public class EmotionalStatePrediction
    {
        [ColumnName("Prediction")]
        public string Prediction { get; set; }
    }
}