// Models/Pregunta.cs
using System;
using System.Collections.Generic;

namespace WebConTablas.Models
{
    public class Pregunta
    {
        public int ID_Pregunta { get; set; }
        public string Contenido { get; set; }
        public string? Extra { get; set; }
        public string? Tipo { get; set; }
        public DateTime? Created_at { get; set; }
        public DateTime? Edited_at { get; set; }

        public ICollection<FormularioPregunta> Formularios { get; set; }
        public ICollection<Respuesta> Respuestas { get; set; } = new List<Respuesta>();
    }
}