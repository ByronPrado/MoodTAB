using Microsoft.Extensions.DependencyInjection;
using MoodTAB.ViewModel;
using MoodTAB.Services;
#if ANDROID
using Android.Content;
using MoodTAB.Platforms.Android;
#endif
namespace MoodTAB.Vistas;

public partial class DiarioPage : ContentPage
{
    private DiarioViewModel viewModel;

    public DiarioPage(IStepCounterService stepService, IDictationService dictationService)
    {
        InitializeComponent();
		viewModel = new DiarioViewModel(stepService, dictationService);
        BindingContext = viewModel;
    }

	// Constructor sin parámetros para Shell/XAML
    public DiarioPage() : this(
        IPlatformApplication.Current.Services.GetRequiredService<IStepCounterService>(),
        IPlatformApplication.Current.Services.GetRequiredService<IDictationService>()) {}
    protected override void OnAppearing()
	{
		base.OnAppearing();
#if ANDROID
		SolicitarPermisosAlIniciar();
#endif
	}
	private async void SolicitarPermisosAlIniciar()
	{
#if ANDROID
		if (!UsageStatsHelper.TienePermisoDeUso())
		{
			bool aceptar = await this.DisplayAlert(
				"Permiso necesario",
				"Para mostrar el tiempo de uso de apps, debes conceder acceso a uso. ¿Deseas abrir la configuración ahora?",
				"Sí", "No");

			if (aceptar)
			{
				UsageStatsHelper.OpenUsageAccessSettings();
			}
		}
#else
		await Task.CompletedTask;
#endif
	}
	private void OnSliderValueChanged(object sender, ValueChangedEventArgs e)
	{
		var slider = (Slider)sender;

		// Redondea al entero más cercano
		int roundedValue = (int)Math.Round(e.NewValue);

		// Solo actualiza si realmente cambió (evita bucles visuales)
		if (slider.Value != roundedValue)
			slider.Value = roundedValue;
	}

}
