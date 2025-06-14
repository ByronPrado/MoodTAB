namespace MoodTAB.Vistas;
using MoodTAB.ViewModel;

public partial class HealthDataPage : ContentPage
{   
    private ViewModel.HealthDataViewModel viewModel;
    public HealthDataPage()
    {
        InitializeComponent();
        viewModel = new ViewModel.HealthDataViewModel();
        BindingContext = viewModel;
    }
}