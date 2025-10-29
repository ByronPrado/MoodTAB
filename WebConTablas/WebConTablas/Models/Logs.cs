namespace WebConTablas.Models
{
    public class Logs
    {
        public int ID_Log { get; set; }
        public int ID_Paciente { get; set; }   // Paciente asociado
        public int ID_Psiquiatra { get; set; } // Psiquiatra asociado
        public Paciente? Paciente { get; set; }
        public Psiquiatra? Psiquiatra { get; set; }
        public string TipoLog { get; set; }    // PlanSeguro, Medicacion, etc...
        public string? Actual { get; set; }    // Modificacion actual
        public string? Anterior { get; set; }  // Log anterior
        public DateTime? Fecha { get; set; }   // Fecha del log
    }
}