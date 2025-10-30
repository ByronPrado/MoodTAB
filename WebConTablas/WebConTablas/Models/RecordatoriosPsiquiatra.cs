namespace WebConTablas.Models
{
    public class RecordatoriosPsiquiatra
    {
        public int ID_RecordatorioPsiquiatra { get; set; }
        public int ID_Psiquiatra { get; set; }
        public Psiquiatra? Psiquiatra { get; set; }
        public string? Titulo { get; set; }
        public string? Descripcion { get; set; }
        public string? Grupo { get; set; } // Opcional: para categorizar recordatorios
        public DateTime? Fecha { get; set; }
        public bool IsAllDay { get; set; } // Si el recordatorio es de todo el día o no.
        public string? Color { get; set; } // Color opcional para el recordatorio

    }
}