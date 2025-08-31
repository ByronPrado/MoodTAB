using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoodTAB.Services;
using MoodTAB.Vistas;
using System;

namespace MoodTAB.ViewModel
{
    
    
public partial class UsuarioExternoViewModel : ObservableObject
{
    [ObservableProperty] string cuestionarioTexto ="text";
    [ObservableProperty] string comentario = "comentario text";

    private readonly INotificationManagerService _notificationManager;

    public UsuarioExternoViewModel(INotificationManagerService notificationManager)
    {
        _notificationManager = notificationManager;
        //CuestionarioTexto = Globals.cuestionario_pendiente ? Globals.cuestionario : "No hay cuestionarios pendientes";
    }

    [RelayCommand]
    public async Task EnviarComentario()
    {
        if (string.IsNullOrWhiteSpace(Comentario)) return;

        // Simulación de envío a la API
        await Task.Delay(500);
        await Application.Current.MainPage.DisplayAlert("Éxito", "Comentario enviado", "OK");
        Comentario = string.Empty;
    }
}

}