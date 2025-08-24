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
        var userNombre = SecureStorage.GetAsync("user_nombre").Result;
        var userEmail = SecureStorage.GetAsync("user_email").Result;

        Globals.nombre_Usuario = userNombre;
        Globals.email_Usuario = userEmail;
        Globals.id_paciente_DB = userId;
        if (!string.IsNullOrEmpty(userId))
        {
            // Usuario autenticado previamente → ir directo a AppShell
            MainPage = new AppShell();
        }
        else
        {
            // Usuario nuevo → pedir login primero
            MainPage = new LoginPage();
        }
    }

}
