using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoodTAB.Models;
using MoodTAB.Vistas;
using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Net.Http; // <-- Este using debe ir aquí
// using System.Net.Http.Json; // No lo necesitas para GetStringAsync

namespace MoodTAB.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _title = "Bienvenido a MoodTAB";
        private string _nameUser;

        [ObservableProperty]
        private string _titleApi = "Bienvenido a MoodTAB";

        public string NameUser
        {
            get => _nameUser;
            set => SetProperty(ref _nameUser, value);
        }

        public MainViewModel()
        {
            ActualizarDatosUsuario();
            CargarSaludoAsync();
        }

        public void ActualizarDatosUsuario()
        {
            // Inicializar el nombre de usuario
            NameUser = Globals.nombre_Usuario;
            Title = $"Bienvenido a MoodTAB {NameUser}";
        }
        private async void CargarSaludoAsync()
        {
            try
            {
                TitleApi = await ObtenerSaludoAsync();
            }
            catch (Exception ex)
            {
                TitleApi = $"Error: {ex.Message}";
            }
        }

        public async Task<string> ObtenerSaludoAsync()
        {
            using var client = new HttpClient();
            var url = "http://10.0.2.2:5051/api/pacientes";
            return await client.GetStringAsync(url);
        }

        [RelayCommand]
        private async Task MovetoPage(string pageName)
        {
            try
            {
                // Usamos nameof(ClaseDeLaPagina) como identificador
                actualizarNombreUsuario();
                await Shell.Current.GoToAsync(pageName);
            }
            catch (Exception ex)
            {
                Title = $"Error al navegar: {ex.Message}";
            }
        }
        private void actualizarNombreUsuario()
        {
            // Actualizar el nombre de usuario en la vista
            NameUser = Globals.nombre_Usuario;
            Title = $"Bienvenido a MoodTAB {NameUser}";
        }
    }
}