namespace MoodTAB.Vistas;

using MoodTAB.ViewModel;
using MoodTAB.ViewModels;

public partial class UserPage : ContentPage
{
	public UserPage()
	{
		InitializeComponent();
		BindingContext = new UserViewModel();
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