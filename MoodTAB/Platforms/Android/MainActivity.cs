using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Speech;
using MoodTAB.Services;

namespace MoodTAB;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    public static MainActivity? Instance { get; private set; }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        Instance = this;
        CreateNotificationFromIntent(Intent);
        Window.SetStatusBarColor(Android.Graphics.Color.ParseColor("#6493e5"));

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
}