using System;
using System.Collections.Generic;
using WebConTablas.Models;

namespace WebConTablas.Models
{
    public class FormularioAsignado
    {
        public int ID_Asignacion { get; set; }
        public int ID_Formulario { get; set; }
        public Formulario Formulario { get; set; }

        public int ID_Paciente { get; set; }
        public Paciente Paciente { get; set; }

        public DateTime Fecha_Asignacion { get; set; }
        public DateTime? Fecha_Limite { get; set; }
        public string Estado { get; set; } = "pendiente";

        public ICollection<Respuesta> Respuestas { get; set; } = new List<Respuesta>();
    }
}