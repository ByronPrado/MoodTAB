namespace MoodTAB.Vistas;
using MoodTAB.ViewModel;
using MoodTAB.Models;

public partial class DetalleDiarioPage : ContentPage
{
    private ViewModel.DetalleDiarioViewModel viewModel;
    public DetalleDiarioPage(Diario diario)
    {
        InitializeComponent();
        viewModel = new ViewModel.DetalleDiarioViewModel(diario);
		BindingContext = viewModel;
    }
}