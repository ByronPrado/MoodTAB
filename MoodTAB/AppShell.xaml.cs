namespace MoodTAB;
using MoodTAB.Vistas;
public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
		Routing.RegisterRoute(nameof(DataBasePage), typeof(DataBasePage));
		Routing.RegisterRoute(nameof(AddPreguntaPage), typeof(AddPreguntaPage));
		Routing.RegisterRoute(nameof(CuestionarioPage), typeof(CuestionarioPage));
		Routing.RegisterRoute(nameof(HealthDataPage), typeof(HealthDataPage));
		Routing.RegisterRoute(nameof(DiarioPage), typeof(DiarioPage));
		Routing.RegisterRoute(nameof(ListaDiarioPage), typeof(ListaDiarioPage));
		Routing.RegisterRoute(nameof(TestPage), typeof(TestPage));
		Routing.RegisterRoute(nameof(UserPage), typeof(UserPage));}
}
