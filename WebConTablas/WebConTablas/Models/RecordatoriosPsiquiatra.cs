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

    }
}