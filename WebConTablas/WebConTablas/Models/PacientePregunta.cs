// Models/PacientePregunta.cs
public class PacientePregunta
{
    public int PacienteId { get; set; }
    public required Paciente Paciente { get; set; }

    public int PreguntaId { get; set; }
    public required Pregunta Pregunta { get; set; }
}
