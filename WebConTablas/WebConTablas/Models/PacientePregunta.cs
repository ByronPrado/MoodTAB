using System;
using WebConTablas.Models;

namespace WebConTablas.Models
{
    public class PacientePregunta
    {
        public int PacienteId { get; set; }
        public Paciente Paciente { get; set; }

        public int PreguntaId { get; set; }
        public Pregunta Pregunta { get; set; }
    }
}
