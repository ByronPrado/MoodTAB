namespace MoodTAB.Vistas;

using MoodTAB.Models;
using MoodTAB.ViewModel;


public partial class CuestionarioHistorialPage : ContentPage
{
	private CuestionarioHistorialViewModel viewModel;

	public CuestionarioHistorialPage()
	{
		viewModel = new CuestionarioHistorialViewModel();
		BindingContext = viewModel;
		InitializeComponent();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		
		await viewModel.CargarCuestionariosCompletados();
	}

	private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (e.CurrentSelection.Count == 0)
			return;

		var seleccionado = e.CurrentSelection[0] as CuestionarioCompletado;
		if (seleccionado == null)
			return;

	await Navigation.PushAsync(new CuestionarioRespondidoPage(seleccionado));

		((CollectionView)sender).SelectedItem = null;
	}


}