namespace MoodTAB.Vistas;
using MoodTAB.ViewModel;
using MoodTAB.Models;
using MoodTAB.Services;
using Plugin.Maui.Calendar.Enums;


public partial class CalendarioDiario : ContentPage
{
	private CalendarioViewModel viewModel;

	public CalendarioDiario()
	{
		InitializeComponent();
		viewModel = new CalendarioViewModel();
		BindingContext = viewModel;
	}
	private async void OnNewSeleccionado(object sender, EventArgs e)
	{
		try
		{
			await Navigation.PushAsync(new DiarioPage());
		}
		catch (Exception ex)
		{
			Console.WriteLine($"error: {ex}");
			//ErrorLabel.Text = $"Error: {ex.Message}";
			//ErrorLabel.IsVisible = true;
		}
	}

	private void OnCambiarVista(object sender, EventArgs e)
	{
		MiCalendario.CalendarLayout = MiCalendario.CalendarLayout switch
		{
			WeekLayout.Week => WeekLayout.TwoWeek,
			WeekLayout.TwoWeek => WeekLayout.Month,
			WeekLayout.Month => WeekLayout.Week,
			_ => WeekLayout.Week
		};
		CambiarVistaCalendarioButton.Text = CambiarVistaCalendarioButton.Text switch
		{
			"Ver Ultima Semana" => "Ver Ultimos 15 Días",
			"Ver Ultimos 15 Días" => "Ver Mes Completo",
			"Ver Mes Completo" => "Ver Ultima Semana" ,
			_ => "Ver Ultimos 15 Días" 
		};
		//LabelVista.Text = MiCalendario.CalendarLayout.ToString();
	}

}