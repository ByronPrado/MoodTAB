namespace MoodTAB;

using MoodTAB.Data;
using MoodTAB.Vistas;

public partial class App : Application
{
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

        if (!string.IsNullOrEmpty(userId))
        {
            // Usuario autenticado previamente → ir directo a AppShell
            MainPage = new AppShell();
        }
        else
        {
            // Usuario nuevo → pedir login primero
            MainPage = new NavigationPage(new LoginPage());
        }
    }
    
    public static IServiceProvider ServiceProvider => Current?.Handler?.MauiContext?.Services
        ?? throw new InvalidOperationException("No se encontró el servicio. Asegúrate de que MauiContext esté configurado correctamente.");
}
