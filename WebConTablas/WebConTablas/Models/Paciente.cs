// Models/Paciente.cs
using System.Collections.Generic;

public class Paciente
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public int IdPsiquiatra { get; set; }

    public required ICollection<PacientePregunta> PacientePreguntas { get; set; }

    public Paciente()
    {
        PacientePreguntas = new List<PacientePregunta>();
    }
}