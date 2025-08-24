namespace MoodTAB.Vistas;

using MoodTAB.ViewModel;
using MoodTAB.Services;


#if ANDROID
using Android;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using MoodTAB.Platforms.Android;
#endif

public partial class MainPage : ContentPage
{

    INotificationManagerService notificationManager;
    private ViewModel.MainViewModel viewModel;
    public MainPage(INotificationManagerService manager)
    {
        InitializeComponent();
        viewModel = new ViewModel.MainViewModel();
        BindingContext = viewModel;

        notificationManager = manager;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
#if ANDROID
        PermissionStatus status = await Permissions.RequestAsync<MoodTAB.Platforms.Android.NotificationPermission>();
        RequestActivityRecognitionPermission();
        viewModel.ActualizarDatosUsuario();
#endif
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
        string message = $"Han pasado 10 min";
        notificationManager.SendNotification(title, message, DateTime.Now.AddSeconds(10));
    }

    
}