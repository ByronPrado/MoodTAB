// Models/Paciente.cs
using System;
using System.Collections.Generic;

namespace WebConTablas.Models
{
    public class Paciente
    {
        public int ID_Paciente { get; set; }
        public string Nombre { get; set; }
        public string? Diagnostico { get; set; }
        public int Edad { get; set; }
        public string? Sexo { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public string Contrasena { get; set; }
        public int? ID_Psiquiatra { get; set; }
        public Psiquiatra? Psiquiatra { get; set; }
        public string? PlanSeguro { get; set; }
        public string? Severidad { get; set; }

        public ICollection<FormularioAsignado> FormulariosAsignados { get; set; } = new List<FormularioAsignado>();
        public ICollection<DiarioEmocional> DiariosEmocionales { get; set; } = new List<DiarioEmocional>();
        public ICollection<UsuarioExterno> UsuariosExternos { get; set; } = new List<UsuarioExterno>();
        public ICollection<Alertas> Alertas { get; set; } = new List<Alertas>();
        public ICollection<Logs> Logs { get; set; } = new List<Logs>();

        public static implicit operator Paciente(PacienteDto v)
        {
            throw new NotImplementedException();
        }
    }
}
