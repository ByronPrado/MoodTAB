namespace MoodTAB.Vistas;

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