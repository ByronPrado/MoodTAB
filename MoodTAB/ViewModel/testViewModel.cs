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
        [ObservableProperty] private bool healthDataManual = Globals.OptionManual;
        [ObservableProperty] private bool optionSueno = Globals.OptionSueno;
        [ObservableProperty] private bool optionHR = Globals.OptionHR;
        [ObservableProperty] private bool optionHRV = Globals.OptionHVR;
        [ObservableProperty] private bool optionPasos = Globals.OptionPasos;
        public TestViewModel()
        {
            //constructor
            texto = "texto";
        }

        partial void OnHealthDataManualChanged(bool oldValue, bool newValue)
        {
            SecureStorage.SetAsync("healthdata_manual", newValue.ToString());
            Globals.OptionManual = newValue;
        }

        partial void OnOptionSuenoChanged(bool oldValue, bool newValue)
        {
            SecureStorage.SetAsync("ingreso_sueno", newValue.ToString());
            Globals.OptionSueno = newValue;
        }
        partial void OnOptionHRChanged(bool oldValue, bool newValue)
        {
            SecureStorage.SetAsync("ingreso_hr", newValue.ToString());
            Globals.OptionHR = newValue;
        }
        partial void OnOptionHRVChanged(bool oldValue, bool newValue)
        {
            SecureStorage.SetAsync("ingreso_hrv", newValue.ToString());
            Globals.OptionHVR = newValue;
        }
        partial void OnOptionPasosChanged(bool oldValue, bool newValue)
        {
            SecureStorage.SetAsync("ingreso_pasos", newValue.ToString());
            Globals.OptionPasos = newValue;
        }

    }

}