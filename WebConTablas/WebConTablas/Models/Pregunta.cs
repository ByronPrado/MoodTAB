public class Pregunta
{
    public int Id { get; set; }
    public string Contenido { get; set; }

    public int IdPaciente { get; set; }
    public Paciente? Paciente { get; set; }

}
