namespace MoodTAB.Vistas;
using MoodTAB.ViewModel;

public partial class DiarioPage : ContentPage
{
	private ViewModel.DiarioViewModel viewModel;
	public DiarioPage()
	{
		InitializeComponent();
		viewModel = new ViewModel.DiarioViewModel();
		BindingContext = viewModel;
	}
}