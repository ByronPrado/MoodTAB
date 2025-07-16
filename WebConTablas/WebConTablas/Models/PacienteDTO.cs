public class PacienteDTO
{
    public int ID_Paciente { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Diagnostico { get; set; }
    public int Edad { get; set; }
    public string? Sexo { get; set; }
    public string? Email { get; set; }
    public string? Telefono { get; set; }

    public List<DiarioEmocionalDTO> DiariosEmocionales { get; set; } = new List<DiarioEmocionalDTO>();
}
