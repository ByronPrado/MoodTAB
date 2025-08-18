namespace MoodTAB.Vistas;

public partial class UserPage : ContentPage
{
	public UserPage()
	{
		InitializeComponent();
		// aqui me falta  al logica de solicitar los datos del usuario a la app web

	}
	private void onEditClicked(Object sender, EventArgs e)
	{
		//solo pa navegar a la pagina de edicion 
		// dudando si lo hacemos en la misma pagina o no utilizando un booleano para la vista del xaml
		//onda si es edit= false se muestra los datos en label si es true mostramos los entry a modo de edicion.
		Shell.Current.GoToAsync("EditarUsuarioPage");
	}
}