using Microsoft.Extensions.DependencyInjection;
using MoodTAB.ViewModel;
using MoodTAB.Services;
using MoodTAB.Popups;
using CommunityToolkit.Maui.Views;
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
		
		/*if (viewModel.DebeMostrarTutorial)
        {
            this.ShowPopup(new DiarioTutorialPopUp());
        }*/
        this.ShowPopup(new DiarioTutorialPopUp());
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

		int valor = (int)Math.Round(e.NewValue);

		if (slider.Value != valor)
			slider.Value = valor;

		if (valor <= 3)
		{
			slider.ThumbColor = Colors.Red;
			slider.MinimumTrackColor = Colors.Red;
		}
		else if (valor <= 5)
		{
			slider.ThumbColor = Colors.Orange;
			slider.MinimumTrackColor = Colors.Orange;
		}
		else
		{
			slider.ThumbColor = Colors.Green;
			slider.MinimumTrackColor = Colors.Green;
		}
	}

}
