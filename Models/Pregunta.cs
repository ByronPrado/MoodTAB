using SQLite;
namespace MoodTAB.Models;
public class Pregunta
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    [NotNull]
    public int Usuario_dirigido { get; set; }
    [NotNull]
    public string? Texto_Pregunta { get; set; }
    [NotNull]
    public string? Tipo_Pregunta { get; set; }

    public string? Opciones_Seleccion { get; set; }
  
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    }