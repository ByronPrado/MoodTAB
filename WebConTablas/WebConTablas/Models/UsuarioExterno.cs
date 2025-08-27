// Models/  UsuarioExterno.cs
using System;
using System.Collections.Generic;

namespace WebConTablas.Models
{
    public class UsuarioExterno
    {
        public int ID_Paciente { get; set; }
        public int IdUsuarioExterno { get; set; }
        public string Nombre { get; set; }
        public string? Parentezco { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public int? ID_Psiquiatra { get; set; } //Psiquiatra asignado al paciente.
        public Psiquiatra? Psiquiatra { get; set; }
        public Paciente? Paciente { get; set; }

        public ICollection<ComentariosExternos> Comentarios { get; set; } = new List<ComentariosExternos>();
    }
}
