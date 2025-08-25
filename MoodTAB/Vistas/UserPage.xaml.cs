namespace MoodTAB.Vistas;

using MoodTAB.ViewModel;
using MoodTAB.ViewModels;

public partial class UserPage : ContentPage
{
	public UserPage()
	{
		InitializeComponent();
		BindingContext = new UserViewModel();
		// aqui me falta  al logica de solicitar los datos del usuario a la app web

	}

	    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        var loginVM = new LoginViewModel();
        await loginVM.Logout();
    }
}