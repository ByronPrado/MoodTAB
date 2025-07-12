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

    public DiarioPage(IStepCounterService stepService)
    {
        InitializeComponent();
        viewModel = new DiarioViewModel(stepService);
        BindingContext = viewModel;
    }

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
#endif
	}

}
