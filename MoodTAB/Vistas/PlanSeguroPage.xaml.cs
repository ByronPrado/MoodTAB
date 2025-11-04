namespace MoodTAB.Vistas;

using MoodTAB.ViewModel;

public partial class PlanSeguroPage : ContentPage
{
	private readonly PlanSeguroViewModel viewModel = new();

	public PlanSeguroPage()
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await viewModel.CargarAsync();
		//BindingContext = viewModel;

	}
		private async void onEditarPlanSeguroClicked(object sender, EventArgs e)
	{ 
		await Navigation.PushAsync(new EditarPlanSeguroPage());
	}
}