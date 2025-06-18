using System.Collections.Generic;

namespace WebConTablas.Models
{
    public class Psiquiatra
    {
        public int ID_Psiquiatra { get; set; }
        public string Nombre { get; set; }
        public string Contrasena { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }

        public ICollection<Paciente> Pacientes { get; set; } = new List<Paciente>();
        public ICollection<Formulario> Formularios { get; set; } = new List<Formulario>();
    }
}