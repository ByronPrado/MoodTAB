using System;
using System.Collections.Generic;

namespace WebConTablas.Models
{
    public class Formulario
    {
        public int ID_Formulario { get; set; }
        public int ID_Psiquiatra { get; set; }
        public Psiquiatra Psiquiatra { get; set; }
        public string? Titulo { get; set; }
        public string? Descripcion { get; set; }
        public DateTime? Created_at { get; set; }

        public ICollection<FormularioPregunta> Preguntas { get; set; } = new List<FormularioPregunta>();
        public ICollection<FormularioAsignado> FormulariosAsignados { get; set; } = new List<FormularioAsignado>();
    }
}