namespace MoodTAB.Vistas;
using MoodTAB.Models;
using MoodTAB.ViewModel;
public partial class CuestionarioRespondidoPage : ContentPage
{
	public CuestionarioRespondidoPage(CuestionarioCompletado cuestionario)
	{
		InitializeComponent(); 
		BindingContext = new CuestionarioRespondidoViewModel(cuestionario);
	}
}