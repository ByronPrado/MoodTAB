namespace MoodTAB.Vistas;
using MoodTAB.ViewModel;
using MoodTAB.Models;

public partial class DetalleDiarioPage : ContentPage
{
    private ViewModel.DetalleDiaroViewModel viewModel;
    public DetalleDiarioPage(Diario diario)
    {
        InitializeComponent();
        viewModel = new ViewModel.DetalleDiaroViewModel(diario);
		BindingContext = viewModel;
    }
}