using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoodTAB.Models;
using MoodTAB.Vistas;
using MoodTAB.Services;
using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Net.Http; // <-- Este using debe ir aquí
// using System.Net.Http.Json; // No lo necesitas para GetStringAsync

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
            CargarSaludoAsync();
            getCuestionario();
        }

        public void ActualizarDatosUsuario()
        {
            // Inicializar el nombre de usuario
            NameUser = SecureStorage.GetAsync("user_nombre").Result ?? "TestActDatosusuario";
            EmailUsuario = SecureStorage.GetAsync("user_email").Result ?? "test";
            Title = $"Bienvenido a MoodTAB {NameUser}";
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
            }
        }

        public async Task<string> ObtenerSaludoAsync()
        {
            using var client = new HttpClient();
            var url = "http://10.0.2.2:5051/api/pacientes";
            return await client.GetStringAsync(url);
        }

        [RelayCommand]
        private async Task MovetoPage(string pageName)
        {
            try
            {
                // Usamos nameof(ClaseDeLaPagina) como identificador
                ActualizarDatosUsuario();
                await Shell.Current.GoToAsync(pageName);
            }
            catch (Exception ex)
            {
                Title = $"Error al navegar: {ex.Message}";
            }
        }
        private async void getCuestionario()
        {
            var notif = SecureStorage.GetAsync("notif_c").Result;
            var url = $"http://10.0.2.2:5051/api/formulario/{Globals.id_paciente_DB}";
            using var client = new HttpClient();
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                Globals.cuestionario_pendiente = false;
                Globals.cuestionario = "{\"type\":\"https://tools.ietf.org/html/rfc9110#section-15.5.5\",\"title\":\"Not Found\",\"status\":404,\"traceId\":\"00-f96b9cb571508dc30bc3bd3a5f71d6e3-11f9152401b32740-00\"}";
                Cuestionario = false;
            }
            else
            {
                Globals.cuestionario_pendiente = true;
                Globals.cuestionario = await response.Content.ReadAsStringAsync();
                Cuestionario = true;
                
                if (notif == null) {
                    notificationManager.SendNotification(
                        "Cuestionario MoodTAB",
                        "Tienes un cuestionario pendiente por responder.",
                        DateTime.Today.AddHours(23),
                        1001
                    );
                    await SecureStorage.SetAsync("notif_c", "active");
                }
            }

        }
    }
}