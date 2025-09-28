namespace MoodTAB;

using MoodTAB.Data;
using MoodTAB.Vistas;
using MoodTAB.Services;
using MoodTAB.Platforms.Android;
using Microsoft.Maui.Controls;

public partial class App : Application
{
    public static IServiceProvider ServiceProvider => Current?.Handler?.MauiContext?.Services
        ?? throw new InvalidOperationException("No se encontró el servicio. Asegúrate de que MauiContext esté configurado correctamente.");
    static TodoItemDataBase? database;

    public static TodoItemDataBase Database
    {
        get
        {
            if (database == null)
            {
                database = new TodoItemDataBase(Constants.DatabasePath);
            }
            return database;
        }
    }
    public App()
    {
        InitializeComponent();

        // Verificar si el usuario ya tiene sesión guardada
        var userId = SecureStorage.GetAsync("user_id").Result;
        var userNombre = SecureStorage.GetAsync("user_nombre").Result;
        var userEmail = SecureStorage.GetAsync("user_email").Result;
        var esFamiliar = SecureStorage.GetAsync("es_familiar").Result;

        Globals.nombre_Usuario = userNombre;
        Globals.email_Usuario = userEmail;
        Globals.id_paciente_DB = userId;
        Globals.esFamiliar = Globals.toBool(esFamiliar);

        var resp = SecureStorage.GetAsync("resp").Result;
        Globals.respondido = resp != null && Globals.toBool(resp);

        var notificationManager = new NotificationManagerService();
        var dictationService = new DictationService();

        if (!string.IsNullOrEmpty(userId)) // Solo si hay sesión guardada
        {
            if (!Globals.esFamiliar)
            {
                MainPage = new NavigationPage(new MainPage(notificationManager));
            }
            else
            {
                MainPage = new NavigationPage(new UsuarioExternoPage(notificationManager,dictationService));
            }
        }
        else
        {
            // Si no hay sesión, siempre LoginPage
            MainPage = new NavigationPage(new LoginPage());
        }
    }


}
