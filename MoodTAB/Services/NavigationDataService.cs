namespace MoodTAB.Services;
using MoodTAB.Models;

public static class NavigationDataService
{
    // Almacenar datos temporales para pasar entre navegaciones
    public static Diario SelectedDiario { get; set; }
    public static CuestionarioCompletado SelectedCuestionarioCompletado { get; set; }
    //public static CuestionarioData SelectedCuestionarioData { get; set; }
}