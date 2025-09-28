using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoodTAB.Services;
using MoodTAB.Vistas;
using System.Net.Http;
using System.Net.Http.Json;
using System;
using System.Collections.ObjectModel;
using MoodTAB.Services;
using Microsoft.Maui.Controls;


namespace MoodTAB.ViewModel
{


    public partial class UsuarioExternoViewModel : ObservableObject
    {
        public ObservableCollection<string> ListaCuestionarios { get; set; } = new();

        [ObservableProperty] string cuestionarioTexto = "text";
        [ObservableProperty] string log_txt;
        [ObservableProperty] string ide;

        [ObservableProperty] string comentarioExterno = null;

        private readonly HttpClient _httpClient;

        private readonly INotificationManagerService _notificationManager;
        private readonly IDictationService dictationService;

        public UsuarioExternoViewModel(INotificationManagerService notificationManager, IDictationService dictationService)
        {
            //Ide = Globals.id_usuario_externo_DB;
            Ide = SecureStorage.GetAsync("externo_id").Result;
            this.dictationService = dictationService;
            _notificationManager = notificationManager;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri($"{Globals.direccion_ngrok}api/")
            };
            // Simulación de cuestionarios (podrías cargarlos desde la API también)
            ListaCuestionarios.Add("Cuestionario 1");
            ListaCuestionarios.Add("Cuestionario 2");
            ListaCuestionarios.Add("Cuestionario 3");
            //CuestionarioTexto = Globals.cuestionario_pendiente ? Globals.cuestionario : "No hay cuestionarios pendientes";
        }

                [RelayCommand]
        public async Task DictarDiario()
        {
            ComentarioExterno = await dictationService.StartDictationAsync();
        }

        [RelayCommand]
        public async Task EnviarComentario()
        {
            try
            {
                Console.WriteLine($"[DEBUG] Enviando comentario...");
                Console.WriteLine($"[DEBUG] IdUsuarioExterno: {Globals.id_usuario_externo_DB}");
                Console.WriteLine($"[DEBUG] Comentario: {ComentarioExterno}");

                if (string.IsNullOrWhiteSpace(ComentarioExterno))
                {
                    Log_txt = "Comentario vacío, no se envía.";
                    Console.WriteLine("[DEBUG] Comentario vacío, cancelando envío.");
                    return;
                }

                var nuevoComentario = new
                {
                    IdUsuarioExterno = int.Parse(Ide),
                    Comentario = ComentarioExterno,
                    Fecha = DateTime.UtcNow
                };
                var url = "ApiComentariosExternos"; // nombre del controlador sin "Controller"
                Console.WriteLine($"[DEBUG] URL final: {_httpClient.BaseAddress}{url}");

                var response = await _httpClient.PostAsJsonAsync(url, nuevoComentario);

                Console.WriteLine($"[DEBUG] StatusCode: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    Log_txt = "Comentario enviado correctamente";
                    await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Éxito", "Comentario enviado", "OK");
                    ComentarioExterno = string.Empty;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Log_txt = $"Error API: {error}";
                    Console.WriteLine($"[DEBUG] Error API: {error}");
                    await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Error", $"No se pudo enviar: {error}", "OK");
                }
            }
            catch (Exception ex)
            {
                Log_txt = $"Excepción: {ex.Message}";
                Console.WriteLine($"[DEBUG] Excepción: {ex}");
                await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}