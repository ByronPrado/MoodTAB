using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoodTAB.Vistas;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;


namespace MoodTAB.ViewModel
{
    public partial class TestViewModel : ObservableObject
    {
        [ObservableProperty] string texto;
        [ObservableProperty] private bool healthDataManual;
        [ObservableProperty] private bool optionSueno = Globals.OptionSueno;
        public TestViewModel()
        {
            //constructor
            texto = "texto";

            // Cargar el valor almacenado de healthdata_manual
            var storedValue = SecureStorage.GetAsync("healthdata_manual").Result ?? "false";
            HealthDataManual = bool.Parse(storedValue);

        }
        
        partial void OnHealthDataManualChanged(bool oldValue, bool newValue)
        {
            // Guardar el nuevo valor en SecureStorage
            SecureStorage.SetAsync("healthdata_manual", newValue.ToString());
        }

    }

}