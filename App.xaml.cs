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

        //MainPage = new NavigationPage(new MainPage());
        MainPage = new AppShell();
    }
    
    public static IServiceProvider ServiceProvider => Current?.Handler?.MauiContext?.Services
        ?? throw new InvalidOperationException("No se encontró el servicio. Asegúrate de que MauiContext esté configurado correctamente.");
}
