namespace MoodTAB.Vistas;

using MoodTAB.ViewModel;
using MoodTAB.Services;

public partial class CuestionarioPage : ContentPage
{
	private ViewModel.Cuestionario viewModel;
	public CuestionarioPage()
	{
		InitializeComponent();
		viewModel = new ViewModel.Cuestionario();
		BindingContext = viewModel;
	}
}