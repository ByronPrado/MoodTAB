namespace MoodTAB;

using MoodTAB.Data;
using MoodTAB.Vistas;
using MoodTAB.Services;
using MoodTAB.Platforms.Android;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Hosting;

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

    protected override async void OnStart()
    {
        await Globals.InitAsync();
    }


    public App()
    {
        InitializeComponent();
        OnStart();
        //Register Syncfusion<sup>®</sup> license
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JFaF5cXGRCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWH9ed3VTRGhfUkN2XkNWYEg=");
        // Verificar si el usuario ya tiene sesión guardada
        var userId = SecureStorage.GetAsync("user_id").Result;
        var userNombre = SecureStorage.GetAsync("user_nombre").Result;
        var userEmail = SecureStorage.GetAsync("user_email").Result;
        var esFamiliar = SecureStorage.GetAsync("es_familiar").Result;
        var psiID = SecureStorage.GetAsync("psiquiatra_id").Result;

        Globals.nombre_Usuario = userNombre;
        Globals.email_Usuario = userEmail;
        Globals.id_paciente_DB = userId;
        Globals.esFamiliar = Globals.toBool(esFamiliar);
        Globals.id_psiquiatra_DB = psiID;

        var resp = SecureStorage.GetAsync("resp").Result;
        Globals.respondido = resp != null && Globals.toBool(resp);

        var notificationManager = new NotificationManagerService();
        var dictationService = new DictationService();
        
        Page root = string.IsNullOrEmpty(SecureStorage.GetAsync("user_id").Result)
        ? new LoginPage()
        : (Globals.esFamiliar
            ? new UsuarioExternoPage(notificationManager, dictationService)
            : new MainPage(notificationManager));

        MainPage = new NavigationPage(root);
    }
}
