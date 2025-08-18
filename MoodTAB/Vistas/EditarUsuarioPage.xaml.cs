namespace MoodTAB.Vistas;

public partial class EditarUsuarioPage : ContentPage
{
	public EditarUsuarioPage()
	{
		InitializeComponent();
	}
	public void OnGuardarCambiosClicked(object sender, EventArgs e)
	{
		// Aquí agregaré la logica para envar los datos a la bd de la appweb
		
		DisplayAlert("Cambios Guardados", "Los cambios se han guardado correctamente.", "OK");
		Shell.Current.GoToAsync("UserPage"); // Regresa a la página de usuario
	}
}