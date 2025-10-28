namespace MoodTAB.Vistas;

using MoodTAB.ViewModel;

public partial class EditarPlanSeguroPage : ContentPage
{
	public EditarPlanSeguroPage()
	{
		InitializeComponent();
		var vm = new EditarPlanSeguroViewModel();
		BindingContext = vm;
		vm.GuardadoExitoso += async (s, e) =>
		{
			await DisplayAlert("Edicion Completada", "Los cambios se guardaron correctamente.", "OK");
		};
	}
}