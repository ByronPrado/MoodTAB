using SQLite;
namespace MoodTAB.Models;
public class Pregunta
{
    [PrimaryKey, AutoIncrement]
    public int ID_Pregunta { get; set; }
    [NotNull]
    public string? Contenido { get; set; }
    public string? Extra { get; set; }
    [NotNull]
    public string? Tipo { get; set; }
    [NotNull]
    public int Usuario_dirigido { get; set; }
  
    public DateTime? Created_at { get; set; }
    public DateTime? Edited_at { get; set; }
    }