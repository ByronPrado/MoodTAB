namespace MoodTAB.Vistas;
using MoodTAB.ViewModel;
using MoodTAB.Models;
using MoodTAB.Services;


public partial class ListaDiarioPage : ContentPage
{
	private ViewModel.ListaDiarioViewModel viewModel;

	private IStepCounterService stepService;
	public ListaDiarioPage(IStepCounterService stepService)
	{
		InitializeComponent();
		this.stepService = stepService;
		viewModel = new ViewModel.ListaDiarioViewModel();
		BindingContext = viewModel;
	}
	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await viewModel.CargarListaDiarios(); // Carga la lista cada vez que se muestra la página
	}
	private async void OnDiarioSeleccionado(object sender, TappedEventArgs e)
	{
		if (e.Parameter is Diario diarioSeleccionado)
		{
			await Navigation.PushAsync(new DetalleDiarioPage(diarioSeleccionado));
		}
	}
	private async void OnNewSeleccionado(object sender, EventArgs e)
	{
		try
	{
		await Navigation.PushAsync(new DiarioPage(stepService));
	}
	catch (Exception ex)
	{
		ErrorLabel.Text = $"Error: {ex.Message}";
		ErrorLabel.IsVisible = true;
	}
		
	}
}