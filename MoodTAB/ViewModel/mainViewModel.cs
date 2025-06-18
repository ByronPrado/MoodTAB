using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoodTAB.Models;
using MoodTAB.Vistas;
using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MoodTAB.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {


        [ObservableProperty]
        private string _title = "Bienvenido a MoodTAB";
        private string _nameUser;

        public string NameUser
        {
            get => _nameUser;
            set => SetProperty(ref _nameUser, value);
        }


        public MainViewModel()
        {
            // Inicializar el nombre de usuario
            _nameUser = Globals.nombre_usuario;
            _title = $"Bienvenido a MoodTAB {_nameUser}";
        }

        [RelayCommand]
        private async Task MovetoPage(string pageName)
        {
            if (Application.Current?.MainPage?.Navigation != null)
            {
                switch (pageName)
                {
                    case "DataBase":
                        await Application.Current.MainPage.Navigation.PushAsync(new DataBasePage());
                        break;
                    case "Test":
                        await Application.Current.MainPage.Navigation.PushAsync(new TestPage());
                        break;
                    case "AddPregunta":
                        await Application.Current.MainPage.Navigation.PushAsync(new AddPreguntaPage());
                        break;
                    case "Cuestionario":
                        await Application.Current.MainPage.Navigation.PushAsync(new CuestionarioPage());
                        break;
                    case "Diario":
                        try
                        {
                            var diarioPage = App.ServiceProvider.GetRequiredService<DiarioPage>();
                            await Application.Current.MainPage.Navigation.PushAsync(diarioPage);
                            break;
                        }
                        catch (Exception e)
                        {
                            Title = e.Message;
                            break;
                        }

                    default:
                        // Main page en caso de error
                        await Application.Current.MainPage.Navigation.PushAsync(new MainPage());

                        break;
                }
            }
            else
            {
                // Handle the case where navigation is not available
                // For example, show an alert or log an error
            }
        }

    }
}