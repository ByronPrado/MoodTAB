using System;

namespace WebConTablas.Models
{
    public class Respuesta
    {
        public int ID_Respuesta { get; set; }
        public int ID_Asignacion { get; set; }
        public FormularioAsignado FormularioAsignado { get; set; }

        public int ID_Pregunta { get; set; }
        public Pregunta Pregunta { get; set; }

        public string Contenido { get; set; }
        public DateTime? Fecha_Respuesta { get; set; }
    }
}