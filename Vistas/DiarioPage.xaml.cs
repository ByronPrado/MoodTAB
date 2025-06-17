using Microsoft.Extensions.DependencyInjection;
using MoodTAB.ViewModel;
using MoodTAB.Services;

namespace MoodTAB.Vistas;

public partial class DiarioPage : ContentPage
{
    private DiarioViewModel viewModel;

    public DiarioPage(IStepCounterService stepService)
    {
        InitializeComponent();
        viewModel = new DiarioViewModel(stepService);
        BindingContext = viewModel;
    }
}
