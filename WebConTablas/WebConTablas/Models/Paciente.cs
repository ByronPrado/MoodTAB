// Models/Paciente.cs
using System.Collections.Generic;

public class Paciente
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public int IdPsiquiatra { get; set; }

    public List<Pregunta>? Preguntas { get; set; }

}