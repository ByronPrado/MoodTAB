using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp1;
using MoodTAB.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MoodTAB.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
       

        // 3) Título enlazado al Label
        [ObservableProperty]
        private string _title = "MoodTAB";

        

        public MainViewModel()
        {
           
        }      

        [RelayCommand]
        private async Task DatabasePage()
        {
            if (Application.Current?.MainPage?.Navigation != null)
            {
                await Application.Current.MainPage.Navigation.PushAsync(new DataBasePage());
            }
            else
            {
                // Handle the case where navigation is not available
                // For example, show an alert or log an error
            }
        }
        [RelayCommand]
        private async Task TestPage()
        {
            if (Application.Current?.MainPage?.Navigation != null)
            {
                await Application.Current.MainPage.Navigation.PushAsync(new TestPage());
            }
            else
            {
                // Handle the case where navigation is not available
                // For example, show an alert or log an error
            }
        }
    }
}