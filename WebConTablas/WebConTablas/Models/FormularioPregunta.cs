using System;

namespace WebConTablas.Models
{
    public class FormularioPregunta
    {
        public int ID_Formulario { get; set; }
        public Formulario Formulario { get; set; }

        public int ID_Pregunta { get; set; }
        public Pregunta Pregunta { get; set; }

        public int? Orden { get; set; }
    }
}