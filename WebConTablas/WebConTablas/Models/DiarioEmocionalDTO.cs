public class DiarioEmocionalDTO
{
    public int ID_Diario { get; set; }
    public DateTime Fecha { get; set; }
    public string? Emociones { get; set; }
    public int? Pasos { get; set; }
    public double? Horas_celular { get; set; }
    public double? Horas_redes { get; set; }
    public string? Descripcion { get; set; }
    public string? Estado { get; set; }
    public string? Hora_dormida { get; set; }
}
