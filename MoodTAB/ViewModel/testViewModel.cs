using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoodTAB.Vistas;
using Microsoft.Maui.Controls;


namespace MoodTAB.ViewModel
{
    public partial class TestViewModel : ObservableObject
    {   [ObservableProperty] string texto;
        public TestViewModel()
        {
            //constructor
            texto = "texto";

        }

    }

}