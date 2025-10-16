using SQLite;
using System;
namespace MoodTAB.Models;

public class Diario
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [NotNull]
    public string? Emocion_Diaria { get; set; }
    [NotNull]
    public string? Descripcion { get; set; }
    public double Horas_Celular { get; set; }
    public double Horas_Redes { get; set; }
    public int Cantidad_Pasos { get; set; }
    public string? Horas_Sueno { get; set; }
    public int HR_RitmoCardiaco { get; set; }
    public int HRV_VariabilidadFrecuencia { get; set; }
    public TimeSpan Hora_Durmio { get; set; }
    public TimeSpan Hora_Desperto { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
}