namespace MoodTAB;
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
	protected override void OnAppearing()
    {
        base.OnAppearing();

        // Ejecuta el comando que carga todos los ítems de la base de datos
        if (viewModel.LoadItemsCommand.CanExecute(null))
        {
            _ = viewModel.LoadItemsCommand.ExecuteAsync(null);
        }
    }

}

