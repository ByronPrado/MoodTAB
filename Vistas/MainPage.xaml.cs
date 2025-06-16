namespace MoodTAB.Vistas;
using MoodTAB.ViewModel;

public partial class MainPage : ContentPage
{

	private ViewModel.MainViewModel viewModel;
	public MainPage()
	{
		InitializeComponent();
		viewModel = new ViewModel.MainViewModel();
		BindingContext = viewModel;
	}
}