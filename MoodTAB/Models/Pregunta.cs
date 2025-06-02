using SQLite;
namespace MoodTAB.Models;
public class Pregunta
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    [NotNull]
    public string? Texto_Pregunta { get; set; }
  
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}