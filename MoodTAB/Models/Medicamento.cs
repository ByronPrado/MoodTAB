using SQLite;
namespace MoodTAB.Models;
public class Medicamento
{
    [PrimaryKey, AutoIncrement]
    public int ID_Medicamento { get; set; }
    [NotNull]
    public string? Nombre { get; set; }
    public string? Dosis { get; set; }
    [NotNull]
    public int Usuario_dirigido { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    }