using SQLite;
namespace MoodTAB.Models;

public class Respuestas
{
    [PrimaryKey, AutoIncrement]
    public int ID_Respuesta { get; set; }

    [Indexed]
    public int PreguntaId { get; set; } // Foreign key to Pregunta

    [NotNull]
    public string? Texto_Respuesta { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Ignore]
    public Pregunta? Pregunta { get; set; } // Navigation property
    }