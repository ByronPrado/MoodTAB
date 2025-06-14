using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoodTAB.Models;
using MoodTAB.Vistas;
using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MoodTAB.ViewModel;

public partial class HealthDataViewModel : ObservableObject
{ 
    [ObservableProperty]
    private string _title = "Diario de Salud";
    public HealthDataViewModel()
    {

    }

    public void CheckClicked(object sender, EventArgs e)
    {

    }

    public void RequestPermissionsClicked(object sender, EventArgs e)
    {
    }

    public void ReadStepsClicked(object sender, EventArgs e)
    {
    }
    
}