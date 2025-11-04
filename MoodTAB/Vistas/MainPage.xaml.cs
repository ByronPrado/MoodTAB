namespace MoodTAB.Vistas;

using MoodTAB.ViewModel;
using MoodTAB.Services;
using MoodTAB.Popups;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;


#if ANDROID
using Android;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using MoodTAB.Platforms.Android;
using Java.Sql;
#endif

public partial class MainPage : ContentPage
{

    INotificationManagerService notificationManager;
    private MainViewModel viewModel;
    public MainPage(INotificationManagerService manager)
    {
        InitializeComponent();
        viewModel = new ViewModel.MainViewModel(manager);
        viewModel.Navigation = this.Navigation;
        BindingContext = viewModel;

        notificationManager = manager;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        viewModel.getCuestionario();
        //NotificationIdle();
        if (Globals.respondido)
        {
            notificationManager.DeleteNotification(1001);
        }
        /*if (viewModel.DebeMostrarTutorial)
        {
            await Task.Delay(300);
            this.ShowPopup(new TutorialPopup());
        }
        */this.ShowPopup(new TutorialPopup());

#if ANDROID
        PermissionStatus status = await Permissions.RequestAsync<MoodTAB.Platforms.Android.NotificationPermission>();
        RequestActivityRecognitionPermission();
        viewModel.ActualizarDatosUsuario();
#endif
    // Actualizar visibilidad del consejo al entrar en la vista
    var valorGuardado = await SecureStorage.GetAsync("mostrar_comentarios");
    bool mostrarConsejos = true;
    if (bool.TryParse(valorGuardado, out bool result))
        mostrarConsejos = result;

    viewModel.MostrarConsejo = mostrarConsejos;
    }

#if ANDROID
    private void RequestActivityRecognitionPermission()
    {
        if (OperatingSystem.IsAndroidVersionAtLeast(29))
        {
            var permission = Manifest.Permission.ActivityRecognition;

            if (ContextCompat.CheckSelfPermission(Android.App.Application.Context, permission) != (int)Permission.Granted)
            {
                ActivityCompat.RequestPermissions(
                    Platform.CurrentActivity ?? MainActivity.Instance!,
                    new string[] { permission },
                    101);
            }
        }
    }
#endif

    void NotificationClick(object sender, EventArgs e)
    {
        viewModel.ActualizarDatosUsuario();

        string title = $"Notificación de prueba";
        string message = Globals.cuestionario_pendiente.ToString();
        notificationManager.SendNotification(title, message, DateTime.Now.AddSeconds(1), 1);
    }

    void NotificationIdle()
    {
        //viewModel.ActualizarDatosUsuario();

        string title = LabelTituloConsejo.Text ?? "MoodTAB";
        string message = LabelContenidoConsejo.Text ?? "Prueba Consejos";
        notificationManager.SendNotification(title, message, DateTime.Now.AddSeconds(5), 1);
    }

    public async void OnOpinionComentario(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            bool util = button.Text?.Trim().StartsWith("Es util", StringComparison.OrdinalIgnoreCase) ?? false;
            string mensaje = util ? "Se marcó como útil" : "Se marcó como no relevante";

            // persistir la opinión y solicitar siguiente consejo desde el ViewModel
            try
            {
                await viewModel.MarcarConsejoUtilAsync(util);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error marcando opinión: {ex.Message}");
            }

            await Toast.Make(mensaje, ToastDuration.Short, 14).Show();
        }

    }
    private void ShowLoading(bool show)
    {
        LoadingOverlay.IsVisible = show;
    }
}