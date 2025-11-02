namespace MoodTAB;

using MoodTAB.Vistas;
public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(AddPreguntaPage), typeof(AddPreguntaPage));
		Routing.RegisterRoute(nameof(BorrarDatosPage), typeof(BorrarDatosPage));
		Routing.RegisterRoute(nameof(CalendarioDiario), typeof(CalendarioDiario));
		Routing.RegisterRoute(nameof(CuestionarioDetallePage), typeof(CuestionarioDetallePage));
		Routing.RegisterRoute(nameof(CuestionarioHistorialPage), typeof(CuestionarioHistorialPage));
		Routing.RegisterRoute(nameof(CuestionarioPage), typeof(CuestionarioPage));
		Routing.RegisterRoute(nameof(CuestionarioRespondidoPage), typeof(CuestionarioRespondidoPage));
		Routing.RegisterRoute(nameof(DataBasePage), typeof(DataBasePage));
		Routing.RegisterRoute(nameof(DetalleDiarioPage), typeof(DetalleDiarioPage));
		Routing.RegisterRoute(nameof(DiarioPage), typeof(DiarioPage));
		Routing.RegisterRoute(nameof(EditarPlanSeguroPage), typeof(EditarPlanSeguroPage));
		Routing.RegisterRoute(nameof(HealthDataPage), typeof(HealthDataPage));
		Routing.RegisterRoute(nameof(ListaDiarioPage), typeof(ListaDiarioPage));
		Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
		Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
		Routing.RegisterRoute(nameof(PastilleroPage), typeof(PastilleroPage));
		Routing.RegisterRoute(nameof(PlanSeguroPage), typeof(PlanSeguroPage));
		Routing.RegisterRoute(nameof(TestPage), typeof(TestPage));
		Routing.RegisterRoute(nameof(UserPage), typeof(UserPage));
		Routing.RegisterRoute(nameof(UsuarioExternoPage), typeof(UsuarioExternoPage));
		//Routing.RegisterRoute(nameof(), typeof());


}
}
