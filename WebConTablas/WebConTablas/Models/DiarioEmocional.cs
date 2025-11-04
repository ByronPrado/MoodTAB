using System;
using System.ComponentModel.DataAnnotations.Schema; // Añadir si se usa para la DB

namespace WebConTablas.Models
{
    public class DiarioEmocional
    {
        // Propiedades de la Base de Datos (Clave Primaria y Relaciones)
        public int ID_Diario { get; set; }
        public int ID_Paciente { get; set; }
        public Paciente? Paciente { get; set; }
        public DateTime Fecha { get; set; }
        
        // Propiedad de texto principal (Diario de texto)
        public string? Descripcion { get; set; } 
        
        // Estado predicho por ML.NET
        public string Estado { get; set; } = "basal"; 

        // ----------------------------------------------------------------------
        // PROPIEDADES RECIBIDAS DEL FORMULARIO WEB (DTO)
        // ----------------------------------------------------------------------
        
        // Biometría (Recibidas del formulario)
        public float? HR_RitmoCardiaco { get; set; }
        public float? HRV_VariabilidadFrecuencia { get; set; }
        public int? Pasos { get; set; } // PasosDiarios

        public float? Hora_dormida { get; set; } 
        public int? Horas_celular { get; set; } 
        public int? Horas_redes { get; set; }

        // 2. Emociones (Cadena de 5 scores de sliders)
        public string? Emociones { get; set; }  

        // ----------------------------------------------------------------------
        // PROPIEDADES CALCULADAS (o usadas en ML.NET)
        // ----------------------------------------------------------------------
        
        // Estos se llenan con la simulación en el controlador
        public float? Coherencia { get; set; }
        public float? Errores_gramaticales { get; set; }
        public DateTime? Ultima_Revision_Psiquiatra { get; set; }
    }
}