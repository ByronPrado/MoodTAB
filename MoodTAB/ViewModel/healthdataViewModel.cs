using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
#if ANDROID
using Android.Content;
using Com.Example.Healthbridge;
#endif
using System.Threading.Tasks;
using Microsoft.Maui.Controls;


namespace MoodTAB.ViewModel;

public partial class HealthDataViewModel : ObservableObject
{
    [ObservableProperty]
    private string _title = "Diario de Salud";

    [ObservableProperty]
    private string _stepsText = "?";

    [ObservableProperty]
    private string _sleepText = "?";

    public HealthDataViewModel()
    {
        // Constructor vacío, no necesitamos inicializar nada más
    }

    [RelayCommand]
    public async Task LoadHealthDataAsync()
    {
        StepsText = "Cargando...";
        SleepText = "Cargando...";

#if ANDROID
        var context = Android.App.Application.Context;

        // Ejecutamos en un hilo de background para no bloquear la UI
        var stepsToday = await Task.Run(() =>
            HealthBridge.GetStepsTodayBlocking(context)
        
        );

        var sleepMinutesToday = await Task.Run(() =>
            HealthBridge.GetSleepMinutesTodayBlocking(context)
        );

        StepsText = stepsToday.ToString();
        SleepText = sleepMinutesToday.ToString();
#endif
    }
}
