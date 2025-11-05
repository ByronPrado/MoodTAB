using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Core.Hosting;
using MoodTAB.Vistas;
using MoodTAB.Services;
using CommunityToolkit.Maui;

#if ANDROID
	using MoodTAB.Platforms.Android;
#endif

namespace MoodTAB;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureSyncfusionCore()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});
		//Register Syncfusion<sup>®</sup> license
		Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JFaF5cXGRCf1NpQnxbf1x1ZFBMZFpbRXFPIiBoS35Rc0VqWn9fd3BTR2dYVUZ3VEFc");
#if DEBUG
		builder.Logging.AddDebug();
		builder.Services.AddSingleton(Connectivity.Current);
		builder.Services.AddSingleton<ViewModel.MainViewModel>();
		builder.Services.AddSingleton<MainPage>();

		builder.Services.AddTransient<ViewModel.DataBaseViewModel>();
		builder.Services.AddTransient<DataBasePage>();
#endif

#if ANDROID
		builder.Services.AddSingleton<IStepCounterService, StepCounterService>();
		builder.Services.AddSingleton<IDictationService, DictationService>();
		builder.Services.AddTransient<DiarioPage>();
		builder.Services.AddTransient<ListaDiarioPage>();
		builder.Services.AddSingleton<INotificationManagerService, NotificationManagerService>();
		builder.Services.AddSingleton<IHealthDataService, HealthDataService>();
#endif

		return builder.Build();
	}
}
