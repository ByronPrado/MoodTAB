namespace WebConTablas.ViewModels
{
    public class PacienteDetalleViewModel
    {
        public PacienteDTO Paciente { get; set; }

        public int ID_Paciente => Paciente?.ID_Paciente ?? 0;
        public string Nombre => Paciente?.Nombre ?? string.Empty;
        public string? Diagnostico => Paciente?.Diagnostico;
        public int Edad => Paciente?.Edad ?? 0;
        public string? Sexo => Paciente?.Sexo;
        public string? Email => Paciente?.Email;
        public string? Telefono => Paciente?.Telefono;

        public List<DiarioEmocionalDTO> DiariosEmocionales => Paciente?.DiariosEmocionales ?? new List<DiarioEmocionalDTO>();

        public List<DiarioEmocionalDTO> DiariosRecientes { get; set; } = new List<DiarioEmocionalDTO>();


        public HashSet<string> EmocionesSet { get; set; } = new HashSet<string>();
        public Dictionary<string, int> ConteoEmociones { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, List<double>> EmocionesSuavizadas { get; set; } = new Dictionary<string, List<double>>();
        public List<string> FechasTendencia { get; set; } = new List<string>();

        public double MediaPasosInhibido { get; set; }
        public double MediaPasosExaltado { get; set; }

        public PacienteDetalleViewModel() { }

        public PacienteDetalleViewModel(PacienteDTO paciente)
        {
            Paciente = paciente;
        }
    }
}
