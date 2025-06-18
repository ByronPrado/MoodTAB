namespace MoodTAB.Vistas;
using MoodTAB.ViewModel;

public partial class AddPreguntaPage : ContentPage
{	
	private ViewModel.AddPregunta viewModel;
	public AddPreguntaPage()
	{
		InitializeComponent();
		viewModel = new ViewModel.AddPregunta();
		BindingContext = viewModel;
	}
}