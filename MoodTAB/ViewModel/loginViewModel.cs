using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoodTAB.Services;
using MoodTAB.ViewModel;
using MoodTAB.Vistas;
namespace MoodTAB.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty] string nombre;
        [ObservableProperty] string email;
        [ObservableProperty] string errorMessage;
        [ObservableProperty] string logsMessage;

        private readonly AuthService _authService;

        public LoginViewModel()
        {
            _authService = new AuthService(); //  aquí podrías inyectar por DI
        }

        [RelayCommand]
        public async Task Login()
        {
            LogsMessage = $"Intentando login con Nombre={Nombre}, Email={Email}";

            ErrorMessage = string.Empty;
            Console.WriteLine($"Intentando login con Nombre={Nombre}, Email={Email}");
            var success = await _authService.LoginAsync(Nombre, Email);
            LogsMessage += $"\nResultado login: {success.log}";
            if (success.success)
            {
                // Guarda los datos globales
                Globals.nombre_Usuario =success.user.Nombre;
                Globals.email_Usuario = success.user.Email;
                Globals.id_paciente_DB= success.user.ID_Paciente.ToString(); // Guarda el ID del usuario
                   // Guarda en SecureStorage
                await SecureStorage.SetAsync("user_id", success.user.ID_Paciente.ToString());
                await SecureStorage.SetAsync("user_nombre", success.user.Nombre);
                await SecureStorage.SetAsync("user_email", success.user.Email);

                var notificationManager = App.ServiceProvider.GetService<INotificationManagerService>();

                Application.Current.MainPage = new AppShell()
                { 
                    BindingContext = new MainViewModel(notificationManager)
                };
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

            // Opcional: limpia SecureStorage si lo usas
            SecureStorage.Remove("user_id");
            SecureStorage.Remove("user_nombre");
            SecureStorage.Remove("user_email");

            // Navega a la página de login
            Application.Current.MainPage = new LoginPage();
        }
    }
}
