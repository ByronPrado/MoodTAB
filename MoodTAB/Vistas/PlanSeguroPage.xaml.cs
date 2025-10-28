namespace MoodTAB.Vistas;

using MoodTAB.ViewModel;

public partial class PlanSeguroPage : ContentPage
{
	private PlanSeguroViewModel viewModel;

	public PlanSeguroPage()
	{
		InitializeComponent();
		viewModel = new PlanSeguroViewModel();

		BindingContext = viewModel;
	}
	protected override async void OnAppearing()
    {
		base.OnAppearing();
		await viewModel.InicializarConsejosPlanSeguroAsync();
		await viewModel.CargarAsync();

    }
}