using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoodTAB.Platforms.Android;
using MoodTAB.Services;
using MoodTAB.ViewModel;
using MoodTAB.Vistas;
using Microsoft.Maui.Controls;


namespace MoodTAB.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty] string password;
        [ObservableProperty] string email;
        [ObservableProperty] string errorMessage;
        [ObservableProperty] string logsMessage;
        [ObservableProperty] bool esFamiliar;

        private readonly AuthService _authService;
        private readonly IDictationService dictationService;

        public LoginViewModel()
        {
            _authService = new AuthService(); //  aquí podrías inyectar por DI
            dictationService = new DictationService();
        }

        [RelayCommand]
        public async Task Login()
        {
            LogsMessage = $"Intentando login con Email={Email}";

            ErrorMessage = string.Empty;
            Console.WriteLine($"Intentando login con Email={Email}");
            var success = await _authService.LoginAsync(Email,Password, EsFamiliar);
            LogsMessage += $"\nResultado login: {success.log}";
            if (success.success)
            {
                // Guarda los datos globales
                Globals.nombre_Usuario = success.user.Nombre;
                Globals.email_Usuario = success.user.Email;
                Globals.esFamiliar = EsFamiliar;
                Globals.id_paciente_DB = success.user.ID_Paciente.ToString();
                Globals.id_usuario_externo_DB = success.user.IdUsuarioExterno.ToString();
                // Guarda en SecureStorage
                await SecureStorage.SetAsync("user_id", success.user.ID_Paciente.ToString());
                await SecureStorage.SetAsync("externo_id", success.user.IdUsuarioExterno.ToString());
                await SecureStorage.SetAsync("user_nombre", success.user.Nombre);
                await SecureStorage.SetAsync("user_email", success.user.Email);
                await SecureStorage.SetAsync("es_familiar", EsFamiliar.ToString());

                var notificationManager = App.ServiceProvider.GetService<INotificationManagerService>();
                if (EsFamiliar)
                {
                    Console.WriteLine($"[DEBUG] Obtenido idexterno={success.user.IdUsuarioExterno}");

                    //await SecureStorage.SetAsync("user_id", success.user.ID_Paciente.ToString());
                    Microsoft.Maui.Controls.Application.Current.MainPage = new NavigationPage(new UsuarioExternoPage(notificationManager, dictationService));
                }
                else
                {
                    Console.WriteLine($"[DEBUG] Obtenido idpaciente={success.user.ID_Paciente}");
                    Application.Current.MainPage = new NavigationPage(new MainPage(notificationManager));
                }
            }
            else
            {
                ErrorMessage = "Nombre o email inválidos.";
                Console.WriteLine($"Error: {ErrorMessage}");
            }
        }

        [RelayCommand]
        public async Task Logout()
        {
            await _authService.LogoutAsync();

            // Limpia variables globales
            Globals.nombre_Usuario = null;
            Globals.email_Usuario = null;
            Globals.id_paciente_DB = "0";
            Globals.id_usuario_externo_DB = "0";

            // Opcional: limpia SecureStorage si lo usas
            SecureStorage.Remove("user_id");
            SecureStorage.Remove("user_nombre");
            SecureStorage.Remove("user_email");
            SecureStorage.Remove("externo_id");

            // Navega a la página de login
            //Application.Current.MainPage = new LoginPage();
            Microsoft.Maui.Controls.Application.Current.MainPage = new NavigationPage(new LoginPage());

        }
    }
}
