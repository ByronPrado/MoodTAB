using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoodTAB.Models;
using MoodTAB.Vistas;
using MoodTAB.Services;
using System.Text.Json;
using MoodTAB.Platforms.Android;
using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Net.Http;

using Microsoft.Maui.Controls;


namespace MoodTAB.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly INotificationManagerService notificationManager;
        [ObservableProperty]
        private string _title = "Bienvenido a MoodTAB";
        private string _nameUser = "usuario";
        private string _emailUsuario = "mail";
        [ObservableProperty]
        private bool cuestionario = false;

        [ObservableProperty]
        private string _titleApi = "Bienvenido a MoodTAB";

        public string NameUser
        {
            get => _nameUser;
            set => SetProperty(ref _nameUser, value);
        }
        public string EmailUsuario
        {
            get => _emailUsuario;
            set => SetProperty(ref _emailUsuario, value);
        }
        public MainViewModel(INotificationManagerService notificationManager)
        {
            this.notificationManager = notificationManager;
            ActualizarDatosUsuario();
            try
            {
                CargarSaludoAsync();
                getCuestionario();
            }
            catch (Exception e)
            {
                TitleApi = "No se pudo conectar a la web " + e.Message;
            }

        }

        public void ActualizarDatosUsuario()
        {
            // Inicializar el nombre de usuario
            NameUser = SecureStorage.GetAsync("user_nombre").Result ?? "TestActDatosusuario";
            EmailUsuario = SecureStorage.GetAsync("user_email").Result ?? "test";
            try { Title = $"Hola {NameUser}"; }
            catch (Exception ex)
            {
                Console.WriteLine($"[DEBUG]ERROR mainviewmodel: {ex.Message}");
            }
        }
        private async void CargarSaludoAsync()
        {
            try
            {
                TitleApi = await ObtenerSaludoAsync();
            }
            catch (Exception ex)
            {
                TitleApi = $"Error: {ex.Message}";
                Console.WriteLine($"[DEBUG]ERROR mainviewmodel: {ex.Message}");
            }
        }

        public async Task<string> ObtenerSaludoAsync()
        {
            try
            {
                using var client = new HttpClient
                {
                    Timeout = TimeSpan.FromSeconds(10)
                };
                var url = $"{Globals.direccion_ngrok}api/pacientes";
                return await client.GetStringAsync(url);
            }
            catch (TaskCanceledException ex)
            {
                // Esto ocurre si se supera el Timeout
                TitleApi = "Error: La petición al servidor tardó demasiado (timeout).";
                Console.WriteLine($"Timeout: {ex.Message}");
                return "Error: Timeout";
            }
            catch (HttpRequestException ex)
            {
                TitleApi = "Error de conexión con el servidor.";
                Console.WriteLine($"Http error: {ex.Message}");
                return "Error: Conexión fallida";

            }
            catch (Exception ex)
            {
                TitleApi = $"Error inesperado: {ex.Message}";
                Console.WriteLine($"Excepción inesperada: {ex}");
                return $"Error inesperado: {ex.Message}";
            }

        }

        public INavigation? Navigation { get; set; }

        [RelayCommand]
        private async Task MovetoPage(string pageName)
        {
            try
            {
                ActualizarDatosUsuario();
                Page? page = pageName switch
                {
                    "CuestionarioPage" => new CuestionarioPage(),
                    "TestPage" => new TestPage(),
                    "DiarioPage" => new DiarioPage(),
                    "CalendarioPage" => new CalendarioDiario(),
                    "UserPage" => new UserPage(),
                    _ => null
                };

                if (page != null && Navigation != null)
                    await Navigation.PushAsync(page);
                else
                    Title = $"Error: Página no encontrada o Navigation es null {pageName}";
            }
            catch (Exception ex)
            {
                Title = $"Error al navegar: {ex.Message}";
                Console.WriteLine($"[DEBUG]ERROR Inavigation: {ex.Message}");
            }
        }
        private async void getCuestionario()
        {
            try
            {
                var notif = SecureStorage.GetAsync("notif_c").Result;
                var url = $"{Globals.direccion_ngrok}api/formulario/{Globals.id_paciente_DB}";
                using var client = new HttpClient()
                {
                    Timeout = TimeSpan.FromSeconds(10)
                };
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    Globals.cuestionario_pendiente = false;
                    Globals.cuestionario = "{\"status\":404,\"title\":\"Not Found\"}";
                    Cuestionario = false;
                    return;
                }

                var content = await response.Content.ReadAsStringAsync();
                Globals.cuestionario = content;

                try
                {
                    var cuestionarios = JsonSerializer.Deserialize<List<CuestionarioData>>(content);

                    if (cuestionarios != null && cuestionarios.Any())
                    {
                        Globals.cuestionario_pendiente = true;
                        Cuestionario = true;

                        if (notif == null)
                        {
                            notificationManager.SendNotification(
                                "Cuestionario MoodTAB",
                                "Tienes un cuestionario pendiente por responder.",
                                DateTime.Today.AddHours(23),
                                1001
                            );
                            await SecureStorage.SetAsync("notif_c", "active");
                        }
                    }
                    else
                    {
                        // Lista vacía -> no hay cuestionarios
                        Globals.cuestionario_pendiente = false;
                        Cuestionario = false;
                    }
                }
                catch (Exception ex)
                {
                    // Si no se puede parsear, tratamos como "sin cuestionarios"
                    Globals.cuestionario_pendiente = false;
                    Cuestionario = false;
                    Console.WriteLine($"Error parseando cuestionarios: {ex.Message}");
                }
            }
            catch (TaskCanceledException ex)
            {
                // Esto ocurre si se supera el Timeout
                TitleApi = "Error: La petición al servidor tardó demasiado (timeout).";
                Console.WriteLine($"Timeout: {ex.Message}");
            }
            catch (HttpRequestException ex)
            {
                TitleApi = "Error de conexión con el servidor.";
                Console.WriteLine($"Http error: {ex.Message}");
            }
            catch (Exception ex)
            {
                TitleApi = $"Error inesperado: {ex.Message}";
                Console.WriteLine($"Excepción inesperada: {ex}");
            }
        }
    }
}