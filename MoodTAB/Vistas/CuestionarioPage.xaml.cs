namespace MoodTAB.Vistas;

using MoodTAB.ViewModel;

public partial class CuestionarioPage : ContentPage
{
    private Cuestionario viewModel;

    public CuestionarioPage()
    {
        InitializeComponent();
        viewModel = new Cuestionario();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {    
        await viewModel.InitializeAsync();
        base.OnAppearing();
    }

    private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count == 0)
            return;

        var seleccionado = e.CurrentSelection[0] as CuestionarioData;
        if (seleccionado == null)
            return;

        // Creamos el ViewModel de detalle
        var detalleVm = new CuestionarioDetalleViewmodel();
        detalleVm.SetPreguntas(seleccionado.PreguntasConRespuesta, seleccionado.IdAsignacion);

        // Creamos la página detalle y le pasamos el BindingContext
        var detallePage = new CuestionarioDetallePage
        {
            BindingContext = detalleVm
        };

        // Navegamos con PushAsync (sin depender de Shell.Current.GoToAsync)
        await Navigation.PushAsync(detallePage);

        // Deseleccionamos el item para que pueda seleccionarse otra vez
        ((CollectionView)sender).SelectedItem = null;
    }

    private async void OnVerCompletadosClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CuestionarioHistorialPage());
    }
}
