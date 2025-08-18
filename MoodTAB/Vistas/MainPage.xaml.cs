namespace MoodTAB.Vistas;
using MoodTAB.ViewModel;

#if ANDROID
using Android;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using AndroidX.Core.App;
using AndroidX.Core.Content;
#endif

public partial class MainPage : ContentPage
{

	private ViewModel.MainViewModel viewModel;
	public MainPage()
	{
		InitializeComponent();
		viewModel = new ViewModel.MainViewModel();
		BindingContext = viewModel;
	}
	    protected override void OnAppearing()
    {
        base.OnAppearing();

#if ANDROID
        RequestActivityRecognitionPermission();
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
}