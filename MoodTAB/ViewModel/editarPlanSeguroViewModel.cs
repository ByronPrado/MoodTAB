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

        [ObservableProperty]
        private string numeroEmergencia;


        public EditarPlanSeguroViewModel()
        {
            // Cargar número de emergencia guardado
            NumeroEmergencia = Globals.numeroEmergencia;
        }
        // Comando para agregar un consejo a la lista
        [RelayCommand]
        private void AgregarConsejo()
        {
            if (!string.IsNullOrWhiteSpace(NuevoTitulo) && !string.IsNullOrWhiteSpace(NuevoContenido))
            {
                consejosAgregados.Add(new Globals.ConsejoInfo(NuevoTitulo, NuevoContenido, true));
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
            Globals.numeroEmergencia = NumeroEmergencia; // 👈 Guardamos el número

            try
            {
                var data = new
                {
                    Consejos = updatedDict,
                    NumeroEmergencia
                };
                var json = JsonSerializer.Serialize(data);
                await SecureStorage.SetAsync("plan_seguro_data", json);
                // 👇 Notificar a la vista que se guardó
                GuardadoExitoso?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error guardando Plan Seguro: {ex.Message}");
            }
        }
        // Evento que la vista puede escuchar
        public event EventHandler GuardadoExitoso;
    }
}
