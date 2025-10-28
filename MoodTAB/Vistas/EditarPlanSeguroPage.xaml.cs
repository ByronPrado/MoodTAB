namespace MoodTAB.Vistas;

using MoodTAB.ViewModel;

public partial class EditarPlanSeguroPage : ContentPage
{
	public EditarPlanSeguroPage()
	{
		InitializeComponent();
		BindingContext = new EditarPlanSeguroViewModel();
	}
}