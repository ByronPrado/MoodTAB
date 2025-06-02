namespace MoodTAB;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
		Routing.RegisterRoute(nameof(DataBasePage), typeof(DataBasePage));
		Routing.RegisterRoute(nameof(AddPreguntaPage), typeof(AddPreguntaPage));
		Routing.RegisterRoute(nameof(CuestionarioPage), typeof(CuestionarioPage));
	}
}
