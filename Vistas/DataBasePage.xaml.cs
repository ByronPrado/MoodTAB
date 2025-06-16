namespace MoodTAB.Vistas;
using MoodTAB.ViewModel;


public partial class DataBasePage : ContentPage
{   
    private ViewModel.DataBaseViewModel viewModel;
    public DataBasePage()
    {
        InitializeComponent();
        viewModel = new ViewModel.DataBaseViewModel();
		BindingContext = viewModel;
    }

}