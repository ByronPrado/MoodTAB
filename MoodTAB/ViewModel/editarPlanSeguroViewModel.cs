using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Threading.Tasks;

namespace MoodTAB.ViewModel
{
    public partial class EditarPlanSeguroViewModel : ObservableObject
    {
        // Lista donde se acumulan los consejos que el usuario va agregando
        [ObservableProperty]
        private ObservableCollection<Globals.ConsejoInfo> consejosAgregados = new ObservableCollection<Globals.ConsejoInfo>();

        // Propiedades para binding de Entry/Editor del nuevo consejo
        [ObservableProperty]
        private string nuevoTitulo;

        [ObservableProperty]
        private string nuevoContenido;

        // Comando para agregar un consejo a la lista
        [RelayCommand]
        private void AgregarConsejo()
        {
            if (!string.IsNullOrWhiteSpace(NuevoTitulo) && !string.IsNullOrWhiteSpace(NuevoContenido))
            {
                consejosAgregados.Add(new Globals.ConsejoInfo(NuevoTitulo, NuevoContenido, true));
                
                // Limpiar los campos para poder agregar otro
                NuevoTitulo = string.Empty;
                NuevoContenido = string.Empty;
            }
        }

        // Comando para guardar los consejos agregados
        [RelayCommand]
        private async Task GuardarAsync()
        {
            var updatedDict = new Dictionary<string, Globals.ConsejoInfo>();
            int index = 1;
            foreach (var consejo in ConsejosAgregados)
            {
                updatedDict[index.ToString()] = consejo;
                index++;
            }

            Globals.planSeguroConsejos = updatedDict;

            try
            {
                var json = JsonSerializer.Serialize(updatedDict);
                await SecureStorage.SetAsync("plan_seguro_data", json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error guardando Plan Seguro: {ex.Message}");
            }
        }
    }
}
