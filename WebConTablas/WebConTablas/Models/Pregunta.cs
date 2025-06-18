// Models/Pregunta.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebConTablas.Models
{
    public class Pregunta
    {
        public int ID_Pregunta { get; set; }
        [Required(ErrorMessage = "El contenido es obligatorio")]
        public string Contenido { get; set; }
        public string? Extra { get; set; }
        public string? Tipo { get; set; }
        public DateTime? Created_at { get; set; }
        public DateTime? Edited_at { get; set; }

        // NUEVOS CAMPOS
        public string? OpcionesSeleccion { get; set; } // Para preguntas de selección (separadas por coma)
        public int? EscalaMin { get; set; }            // Para preguntas de escala
        public int? EscalaMax { get; set; }            // Para preguntas de escala

        public ICollection<FormularioPregunta> Formularios { get; set; } = new List<FormularioPregunta>();
        public ICollection<Respuesta> Respuestas { get; set; } = new List<Respuesta>();
    }
}