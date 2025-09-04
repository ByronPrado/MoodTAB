namespace MoodTAB.Vistas;

using MoodTAB.Services;

public partial class BorrarDatosPage : ContentPage
{
	private readonly INotificationManagerService notificationManager;
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
			 try
			{	
				Navigation.PushAsync(new MainPage(notificationManager));
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error: {ex.Message}");
			}

		}
		else
		{
			DisplayAlert("Error", "Debes aceptar la eliminación de datos marcando la casilla.", "OK");
		}
	}
}