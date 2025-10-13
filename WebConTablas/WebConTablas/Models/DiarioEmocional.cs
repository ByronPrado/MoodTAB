using System;

namespace WebConTablas.Models
{
    public class DiarioEmocional
    {
        // Campos existentes
        public int ID_Diario { get; set; }
        public int ID_Paciente { get; set; }
        public Paciente? Paciente { get; set; }
        public DateTime Fecha { get; set; }
        public string? Emociones { get; set; } // Se usa para DeteccionEmociones_Subjetivas
        public string? Descripcion { get; set; } // Se usa para ContenidoSemantico_KeywordsScore y ComplejidadLenguaje_Errores
        public int? Pasos { get; set; } // PasosDiarios
        public int? Horas_celular { get; set; } // RedesSociales_Frecuencia (asunción de mapeo)
        public int? Horas_redes { get; set; } // RedesSociales_Frecuencia (asunción de mapeo)
        public string? Hora_dormida { get; set; } // HorasSueno (necesita conversión)
        public string Estado { get; set; } = "normal"; // Se llenará con la predicción

        // NUEVOS CAMPOS REQUERIDOS POR EL MODELO ML.NET
        // Asumimos que estos campos también son enviados en la petición POST
        public float? HR_RitmoCardiaco { get; set; }
        public float? HRV_VariabilidadFrecuencia { get; set; }
        public float? Temperatura_Desviacion { get; set; }
        public float? ContenidoSemantico_KeywordsScore { get; set; }
        public float? ComplejidadLenguaje_Errores { get; set; }
    }
}