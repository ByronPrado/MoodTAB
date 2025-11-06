using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoodTAB.Vistas;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;


namespace MoodTAB.ViewModel
{
    public partial class TestViewModel : ObservableObject
    {
        [ObservableProperty] string opcionSuenoTexto;
        [ObservableProperty] string opcionSuenoTexto2;
        [ObservableProperty] private bool healthDataManual = Globals.OptionManual;
        [ObservableProperty] private bool optionSueno = Globals.OptionSueno;
        [ObservableProperty] private bool optionHR = Globals.OptionHR;
        [ObservableProperty] private bool optionHRV = Globals.OptionHVR;
        [ObservableProperty] private bool optionPasos = Globals.OptionPasos;
        [ObservableProperty] private bool opcionMostrarConsejos = Globals.OpcionMostrarConsejos;
        public TestViewModel()
        {
            if (optionSueno)
            {
                opcionSuenoTexto = "Actual: Horas Totales";
                opcionSuenoTexto2 = "24 hrs";
            }
            else
            {
                opcionSuenoTexto = "Actual: Rango de horas";
                opcionSuenoTexto2 = "00:00 - 23:59 hrs";
            }
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
            if (newValue)
            {
                OpcionSuenoTexto = "Actual: Horas Totales";
                OpcionSuenoTexto2 = "24 hrs";
            }
            else
            {
                OpcionSuenoTexto = "Actual: Rango de horas";
                OpcionSuenoTexto2 = "00:00 - 23:59 hrs";
            }
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

        partial void OnOpcionMostrarConsejosChanged(bool oldValue, bool newValue)
        {
            SecureStorage.SetAsync("mostrar_comentarios", newValue.ToString());
            Globals.OpcionMostrarConsejos = newValue;
        }
    }

}