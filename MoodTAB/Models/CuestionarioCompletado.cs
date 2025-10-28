namespace MoodTAB.Models
{
    public class CuestionarioCompletado
    {
        public int iD_Asignacion { get; set; }
        public string Estado { get; set; }
        public DateTime Fecha_Asignacion { get; set; }
        public DateTime? Fecha_Limite { get; set; }
        public FormularioCompletado Formulario { get; set; }
    }

    public class FormularioCompletado
    {
        public int iD_Formulario { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public List<PreguntaCompletada> Preguntas { get; set; }
    }

    public class PreguntaCompletada
    {
        public int iD_Pregunta { get; set; }
        public string Contenido { get; set; }
        public string Tipo { get; set; }
        public string Extra { get; set; }
        public string OpcionesSeleccion { get; set; }
        public int? EscalaMin { get; set; }
        public int? EscalaMax { get; set; }
        public string Respuesta { get; set; }
    }
}