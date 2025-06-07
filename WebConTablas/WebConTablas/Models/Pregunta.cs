// Models/Pregunta.cs
using System.Collections.Generic;

public class Pregunta
{
    public int Id { get; set; }
    public required string Texto { get; set; }

    // Relación muchos a muchos con pacientes
    public required ICollection<PacientePregunta> PacientePreguntas { get; set; }
}
