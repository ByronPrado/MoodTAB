namespace MoodTAB.Vistas;
using MoodTAB.ViewModel;
using MoodTAB.Models;
using MoodTAB.Services;
using System.ComponentModel;
using Plugin.Maui.Calendar.Enums;


public partial class PastilleroPage : ContentPage
{
	private PastilleroViewModel viewModel;

	public PastilleroPage()
	{
		InitializeComponent();
		viewModel = new PastilleroViewModel();
		BindingContext = viewModel;
	}

	private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(viewModel.ShownDate))
        {
            FrameRegistro.IsVisible = viewModel.ShownDate.Date == DateTime.Today;
        }
    }

}