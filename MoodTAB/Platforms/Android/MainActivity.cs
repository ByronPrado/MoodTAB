using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Speech;
using MoodTAB.Services;

using AndroidX.Core.View;
using Android.Views;

using AndroidX.Activity.Result;
using MoodTAB.Platforms.Android.Callbacks;
using AndroidX.Health.Connect.Client;
using JObject = Java.Lang.Object;

namespace MoodTAB;

[IntentFilter(new[] { "androidx.health.ACTION_SHOW_PERMISSIONS_RATIONALE" })]
[IntentFilter(new[] { "android.intent.action.VIEW_PERMISSION_USAGE" },
    Categories = new[] { "android.intent.category.HEALTH_PERMISSIONS" })]
[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    private ActivityResultLauncher _permissionRequestLauncher = null!;
    private TaskCompletionSource<JObject?> _permissionRequestCompletedSource;
    public static readonly TimeSpan MaxPermissionRequestDuration = TimeSpan.FromMinutes(1);
    public static MainActivity? Instance { get; private set; }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        Instance = this;
        CreateNotificationFromIntent(Intent);
        Window.SetStatusBarColor(Android.Graphics.Color.ParseColor("#6493e5"));

        ConfigureWindowInsets();
        EnableImmersiveMode();
        ApplyWindowInsets();

        _permissionRequestLauncher = RegisterForActivityResult(
    PermissionController.CreateRequestPermissionResultContract(),
    new AndroidActivityResultCallback(result =>
    {
        Console.WriteLine($"[v0] Permission result received in MainActivity");
        _permissionRequestCompletedSource?.TrySetResult(result);
        _permissionRequestCompletedSource = null;
    }));

    }

    protected override void OnNewIntent(Intent? intent)
    {
        base.OnNewIntent(intent);

        CreateNotificationFromIntent(intent);
    }

    static void CreateNotificationFromIntent(Intent intent)
    {
        if (intent?.Extras != null)
        {
            string title = intent.GetStringExtra(MoodTAB.Platforms.Android.NotificationManagerService.TitleKey);
            string message = intent.GetStringExtra(MoodTAB.Platforms.Android.NotificationManagerService.MessageKey);

            var service = IPlatformApplication.Current.Services.GetService<INotificationManagerService>();
            service.ReceiveNotification(title, message);
        }
    }

    protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
    {
        base.OnActivityResult(requestCode, resultCode, data);

        if (requestCode == 10 && resultCode == Result.Ok && data != null)
        {
            var matches = data.GetStringArrayListExtra(RecognizerIntent.ExtraResults);
            if (matches?.Count > 0)
            {
                MoodTAB.Platforms.Android.DictationService.Current?.DictationResult?.TrySetResult(matches[0]);
            }
        }
    }

    public Task RequestPermission(Java.Lang.Object permission, TaskCompletionSource<JObject?> whenCompletedSource)
    {
        Console.WriteLine($"[v0] RequestPermission called in MainActivity");
        _permissionRequestCompletedSource?.TrySetResult(null);
        _permissionRequestCompletedSource = whenCompletedSource;
        _permissionRequestLauncher.Launch(permission);
        return whenCompletedSource.Task;
    }

    protected override void OnResume()
    {
        base.OnResume();
        EnableImmersiveMode();
    }

    public override void OnWindowFocusChanged(bool hasFocus)
    {
        base.OnWindowFocusChanged(hasFocus);
        if (hasFocus)
        {
            EnableImmersiveMode();
        }
    }

    private void ConfigureWindowInsets()
    {
        if (Window == null) return;

        // Habilitar control sobre áreas del sistema
        Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
        Window.ClearFlags(WindowManagerFlags.TranslucentStatus | WindowManagerFlags.TranslucentNavigation);

        if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
        {
            // Android 11+ - No decorar automáticamente para sistema
            Window.SetDecorFitsSystemWindows(false);
        }
        else if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
        {
            // Android 5.0 - 10 - Usar flags de layout
            Window.DecorView.SystemUiVisibility = (StatusBarVisibility)(
                SystemUiFlags.LayoutStable |
                SystemUiFlags.LayoutHideNavigation |
                SystemUiFlags.LayoutFullscreen
            );
        }
    }

    private void EnableImmersiveMode()
    {
        if (Window == null) return;

        if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
        {
            // Android 11+ (API 30+)
            //Window.SetDecorFitsSystemWindows(false);
            var controller = Window.InsetsController;
            if (controller != null)
            {
                // Ocultar barra de navegación pero mantener barra de estado visible
                controller.Hide(WindowInsets.Type.NavigationBars());
                controller.SystemBarsBehavior = (int)WindowInsetsControllerBehavior.ShowTransientBarsBySwipe;
            }
        }
        else
        {
            // Android 10 y anteriores
            var uiOptions = (int)(
                SystemUiFlags.LayoutStable |
                SystemUiFlags.LayoutHideNavigation |
                SystemUiFlags.LayoutFullscreen |
                SystemUiFlags.HideNavigation |
                SystemUiFlags.ImmersiveSticky
            );
            Window.DecorView.SystemUiVisibility = (StatusBarVisibility)uiOptions;
        }
    }

    private void ApplyWindowInsets()
    {
        var contentView = FindViewById(Android.Resource.Id.Content);
        if (contentView != null)
        {
            ViewCompat.SetOnApplyWindowInsetsListener(contentView, new WindowInsetsListener());
            ViewCompat.RequestApplyInsets(contentView);
        }
    }
}
public class WindowInsetsListener : Java.Lang.Object, IOnApplyWindowInsetsListener
{
    public WindowInsetsCompat OnApplyWindowInsets(Android.Views.View view, WindowInsetsCompat insets)
    {
        // Obtener los insets de las barras del sistema (status bar, navigation bar)
        var systemBars = insets.GetInsets(WindowInsetsCompat.Type.SystemBars());
        var displayCutout = insets.GetInsets(WindowInsetsCompat.Type.DisplayCutout());

        // Calcular padding combinando system bars y cutouts
        var left = Math.Max(systemBars.Left, displayCutout.Left);
        var top = Math.Max(systemBars.Top, displayCutout.Top);
        var right = Math.Max(systemBars.Right, displayCutout.Right);
        var bottom = Math.Max(systemBars.Bottom, displayCutout.Bottom);

        // Aplicar padding al contenido para que no quede detrás de las barras
        view.SetPadding(left, top, right, 0); // No padding bottom para usar espacio completo

        return WindowInsetsCompat.Consumed;
    }
}