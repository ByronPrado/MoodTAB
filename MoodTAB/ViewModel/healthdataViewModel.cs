using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoodTAB.Models;
using MoodTAB.Vistas;
#if ANDROID
using Android.Content;
using Android.App;
#endif
using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MoodTAB.ViewModel;

public partial class HealthDataViewModel : ObservableObject
{ 
    [ObservableProperty]
    private string _title = "Diario de Salud";
    [ObservableProperty]
    private ObservableCollection<string> _appUsageStats = new();

    [ObservableProperty]
    private string _stepsText = "?";
    public HealthDataViewModel(){}    
    
    [RelayCommand]
    public void CheckClicked()
    {

    }
    [RelayCommand]
    public static void OpenUsageAccessSettings()
    {
        #if ANDROID
                var intent = new Intent(Android.Provider.Settings.ActionUsageAccessSettings);
                intent.SetFlags(ActivityFlags.NewTask);
                Android.App.Application.Context.StartActivity(intent);
        #endif
    }
    [RelayCommand]
    public void RequestPermissionsClicked()
    {
#if ANDROID
        UsageStatsHelper.OpenUsageAccessSettings();
#endif
    }
    [RelayCommand]
    public void ReadStepsClicked()
    {

        #if ANDROID
        AppUsageStats.Clear();

        var stats = UsageStatsHelper.GetAppUsageStats();
        foreach (var stat in (stats ?? Enumerable.Empty<KeyValuePair<string, long>>()).OrderByDescending(x => x.Value).Take(10))        {
            var appName = stat.Key;
            var timeMinutes = stat.Value / 60000;
            AppUsageStats.Add($"{appName}: {timeMinutes} min");
        }
        #endif
    }
    
}