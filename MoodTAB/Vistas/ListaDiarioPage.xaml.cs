namespace MoodTAB.Vistas;
using MoodTAB.ViewModel;
using MoodTAB.Models;
using MoodTAB.Services;

public partial class ListaDiarioPage : ContentPage
{
    private ListaDiarioViewModel viewModel;
    private IStepCounterService stepService;

    public ListaDiarioPage(IStepCounterService stepService)
    {
        InitializeComponent();
        this.stepService = stepService;
        viewModel = new ListaDiarioViewModel();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            await viewModel.CargarListaDiarios();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo cargar la lista de diarios: {ex.Message}", "OK");
        }
    }

    private async void OnDiarioSeleccionado(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Diario diarioSeleccionado)
        {
            await Navigation.PushAsync(new DetalleDiarioPage(diarioSeleccionado));
            ((CollectionView)sender).SelectedItem = null; // limpia selección
        }
    }

    private async void OnNewSeleccionado(object sender, EventArgs e)
    {
        try
        {
            await Navigation.PushAsync(new DiarioPage(stepService));
        }
        catch (Exception ex)
        {
            ErrorLabel.Text = $"Error: {ex.Message}";
            ErrorLabel.IsVisible = true;
        }
    }
}
