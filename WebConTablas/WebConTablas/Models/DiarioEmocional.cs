using System;

namespace WebConTablas.Models
{
    public class DiarioEmocional
    {
        public int ID_Diario { get; set; }
        public int ID_Paciente { get; set; }
        public Paciente Paciente { get; set; }
        public DateTime Fecha { get; set; }
        public string? Emociones { get; set; }
        public string? Descripcion { get; set; }
        public int? Pasos { get; set; }
        public int? Horas_celular { get; set; }
        public int? Horas_redes { get; set; }
        public string? Hora_dormida { get; set; }
        public string Estado { get; set; } = "normal";
    }
}