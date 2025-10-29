using System;
using System.Collections.Generic;

namespace WebConTablas.Models
{
    public class Alertas
    {
        public int ID_Alerta { get; set; }
        public int ID_Paciente { get; set; }
        public Paciente? Paciente { get; set; }
        public string? Contenido { get; set; }
        public string? Tipo { get; set; }
        public string? Estado { get; set; }
        public DateTime? Created_at { get; set; }
    }
}