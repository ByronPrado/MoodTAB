using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoodTAB.Models;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace MoodTAB.ViewModel
{
    public partial class CuestionarioHistorialViewModel : ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<CuestionarioCompletado> cuestionariosCompletados = new();

        [ObservableProperty]
        string log = string.Empty;

        //private bool _yaCargado = false;

        public CuestionarioHistorialViewModel()
        {
            //CargarCuestionariosCompletados();        
        }

        public async Task CargarCuestionariosCompletados()
        {
            try
            {
                CuestionariosCompletados.Clear();

                var url = $"{Globals.direccion_ngrok}api/formulario/completados/{Globals.id_paciente_DB}";

                using var client = new HttpClient();

                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    Log = "No se pudo cargar cuestionarios completados.";
                    return;
                }

                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var cuestionarios = JsonSerializer.Deserialize<List<CuestionarioCompletado>>(content,options);

                if (cuestionarios == null || cuestionarios.Count==0)
                {
                    Log = "No hay cuestionarios completados aún.";
                    return;
                }

                foreach (var item in cuestionarios)
                    CuestionariosCompletados.Add(item);
            }
            catch (Exception ex)
            {
                Log = "Error cargando cuestionarios: " + ex.Message;
            }
        }
        [RelayCommand]
        private async Task SeleccionarCuestionario(CuestionarioData value)
        {
            if (value == null) return;

            await Application.Current.MainPage.DisplayAlert("Cuestionario", 
                $"Abrir detalle del cuestionario {value.Titulo}", "OK");

            // Aquí podrías navegar a detalle si implementas esa vista en modo solo-lectura
        }
    }
}
