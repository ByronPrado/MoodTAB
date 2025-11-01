using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Speech;
using MoodTAB.Services;

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
        _permissionRequestLauncher = RegisterForActivityResult(
            HealthPermissionController.CreateRequestPermissionResultContract(),
            new AndroidActivityResultCallback(result => {
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

    public Task RequestPermission(Java.Util.ISet permission, TaskCompletionSource<JObject?> whenCompletedSource)
    {
        Console.WriteLine($"[v0] RequestPermission called in MainActivity with{ permission.Size()} permissions");
        _permissionRequestCompletedSource?.TrySetResult(null);
        _permissionRequestCompletedSource = whenCompletedSource;
        _permissionRequestLauncher.Launch((Java.Lang.Object)permission);
        return whenCompletedSource.Task;
    }
}