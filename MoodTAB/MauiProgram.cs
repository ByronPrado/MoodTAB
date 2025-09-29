using Microsoft.Extensions.Logging;
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
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

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

		builder.Services.AddSingleton<IHealthBridgeService>(provider =>
        {
            var context = Android.App.Application.Context;
            return new HealthBridgeService(context);
        });
#endif

		return builder.Build();
	}
}
