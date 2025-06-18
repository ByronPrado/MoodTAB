using SQLite;
namespace MoodTAB.Models;
public class TodoItem
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    [NotNull]
    public string? Nombre { get; set; }
    
    [NotNull]
    public string? Rut { get; set; }
    
    public bool IsDone { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}