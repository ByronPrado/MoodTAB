using Microsoft.Extensions.Logging;
using MoodTAB.Vistas;
using MoodTAB.Services;

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
		builder.Services.AddTransient<DiarioPage>();
#endif

		return builder.Build();
	}
}
