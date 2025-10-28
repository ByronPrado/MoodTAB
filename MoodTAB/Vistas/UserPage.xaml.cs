namespace MoodTAB.Vistas;

using MoodTAB.Models;
using MoodTAB.Services;
using MoodTAB.ViewModel;
using MoodTAB.ViewModels;

public partial class UserPage : ContentPage
{
	public UserPage()
	{
		InitializeComponent();
		BindingContext = new UserViewModel();
	}

	private async void onEditarPlanSeguroClicked(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new EditarPlanSeguroPage());
	}

	private async void onBorrarDatosClicked(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new BorrarDatosPage());
	}
	private async void OnLogoutClicked(object sender, EventArgs e)
	{
		var loginVM = new LoginViewModel();
		await loginVM.Logout();
	}
}