namespace MoodTAB.Vistas;

using Microsoft.Maui.ApplicationModel; 
using Microsoft.Maui.ApplicationModel.DataTransfer;



public partial class TestPage : ContentPage
{
	public string Nombre { get; set; } = "Test Page";
	IConnectivity connectivity;
	public TestPage()
	{
		InitializeComponent();
		NombreLabel.Text = Nombre; // Muestra el valor inicial
		connectivity = Connectivity.Current;
	}

	public static string DatabasePath
	{
		get
		{
			var path = FileSystem.AppDataDirectory;
			return Path.Combine(path, "MoodTAB.db3");
		}
	}
	public void OnEditarUsuarioClicked(object sender, EventArgs e)
	{ //NAVEGACION A DATOS DEL USUARIO
		Navigation.PushAsync(new UserPage());
	}

	public void OnHealthConnect(object sender, EventArgs e)
	{ //NAVEGACION A DATOS DEL USUARIO
		Navigation.PushAsync(new HealthDataPage());
	}
	public void OnButtonClicked(object sender, EventArgs e)
	{
		if (connectivity.NetworkAccess != NetworkAccess.Internet)
		{
			DisplayAlert("Error", "No hay conexión a Internet.", "OK");
			return;
		}
		NombreLabel.Text = DatabasePath; // Cambia el texto al presionar el botón
		Clipboard.SetTextAsync(NombreLabel.Text); // Copia al portapapeles
		DisplayAlert("Copiado", "La dirección se copió al portapapeles.", "OK");
		try
		{
			var file = DatabasePath;
			if (!File.Exists(file))
			{
				DisplayAlert("Error", "La base de datos no existe.", "OK");
				return;
			}

			Share.RequestAsync(new ShareFileRequest
			{
				Title = "Compartir archivo SQLite",
				File = new ShareFile(file)
			});
		}
		catch (Exception ex)
		{
			DisplayAlert("Error", $"No se pudo compartir el archivo: {ex.Message}", "OK");
		}
	}
}



