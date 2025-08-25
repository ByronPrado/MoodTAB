namespace MoodTAB.Vistas;

public partial class BorrarDatosPage : ContentPage
{
	public bool check_acepto = false;
	public BorrarDatosPage()
	{
		InitializeComponent();
	}
	
	public void OnCheckBoxCheckedChanged(object sender, CheckedChangedEventArgs e)
	{
		check_acepto = e.Value;
	}
	public void OnEnviarSolicitudClicked(object sender, EventArgs e)
	{
		if (check_acepto)
		{
			// Lógica para borrar los datos del usuario
			DisplayAlert("Confirmado", "Tus solicitud de borrar tus datos ha sido enviada .", "OK");
			// Aquí puedes agregar la lógica para borrar los datos del usuario de la base de datos o almacenamiento
		}
		else
		{
			DisplayAlert("Error", "Debes aceptar la eliminación de datos marcando la casilla.", "OK");
		}
	}
}