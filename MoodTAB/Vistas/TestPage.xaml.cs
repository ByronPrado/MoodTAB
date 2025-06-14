namespace MoodTAB.Vistas;

using Microsoft.Maui.ApplicationModel; 
using Microsoft.Maui.ApplicationModel.DataTransfer;
using MoodTAB.Services; // Agrega esta línea
using System;
using System.IO;
using System.Threading.Tasks;



public partial class TestPage : ContentPage
{
	public string Nombre { get; set; } = "Test Page";
	IConnectivity connectivity;
	private readonly ApiService _apiService = new ApiService(); // Instancia del servicio

	public TestPage()
	{
		InitializeComponent();
		NombreLabel.Text = Nombre; // Muestra el valor inicial
		connectivity = Connectivity.Current;
		_ = ProbarComunicacionAsync(); // Llama al método de prueba al abrir la página
	}

	private async Task ProbarComunicacionAsync()
	{
		try
		{
			var preguntas = await _apiService.GetPreguntasAsync();
			await DisplayAlert("Preguntas recibidas", string.Join("\n", preguntas), "OK");
		}
		catch (Exception ex)
		{
			await DisplayAlert("Error", ex.Message, "OK");
		}
	}

	public static string DatabasePath
	{
		get
		{
			var path = FileSystem.AppDataDirectory;
			return Path.Combine(path, "MoodTAB.db3");
		}
	}
	public async void OnButtonClicked(object sender, EventArgs e)
	{
		// Puedes comentar la verificación de conectividad si da falsos negativos en el emulador
		// if (connectivity.NetworkAccess != NetworkAccess.Internet)
		// {
		//     await DisplayAlert("Error", "No hay conexión a Internet.", "OK");
		//     return;
		// }

		try
		{
			var preguntas = await _apiService.GetPreguntasAsync();
			await DisplayAlert("Preguntas recibidas", string.Join("\n", preguntas), "OK");
		}
		catch (Exception ex)
		{
			await DisplayAlert("Error", ex.Message, "OK");
		}

		// Si quieres mantener la funcionalidad de mostrar/copiar la ruta:
		NombreLabel.Text = DatabasePath;
		await Clipboard.SetTextAsync(NombreLabel.Text);
		await DisplayAlert("Copiado", "La dirección se copió al portapapeles.", "OK");
		try
		{
			var file = DatabasePath;
			if (!File.Exists(file))
			{
				await DisplayAlert("Error", "La base de datos no existe.", "OK");
				return;
			}

			await Share.RequestAsync(new ShareFileRequest
			{
				Title = "Compartir archivo SQLite",
				File = new ShareFile(file)
			});
		}
		catch (Exception ex)
		{
			await DisplayAlert("Error", $"No se pudo compartir el archivo: {ex.Message}", "OK");
		}
	}

	public async void OnTestApiClicked(object sender, EventArgs e)
	{
		try
		{
			var preguntas = await _apiService.GetPreguntasAsync();
			await DisplayAlert("Preguntas recibidas", string.Join("\n", preguntas), "OK");
		}
		catch (Exception ex)
		{
			await DisplayAlert("Error", ex.ToString(), "OK"); // Usa ex.ToString() para más detalles
		}
	}
}



