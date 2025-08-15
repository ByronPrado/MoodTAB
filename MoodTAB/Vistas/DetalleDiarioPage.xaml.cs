namespace MoodTAB.Vistas;
using MoodTAB.ViewModel;
using MoodTAB.Models;

public partial class DetalleDiarioPage : ContentPage
{
    public DetalleDiarioPage(Diario diario)
    {
        InitializeComponent();
        BindingContext = diario;
    }
}