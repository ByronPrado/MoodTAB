using CommunityToolkit.Mvvm.ComponentModel;
using MoodTAB;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace MoodTAB.ViewModel
{
    public partial class PlanSeguroViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<Globals.ConsejoInfo> consejosPlanSeguro;

        public PlanSeguroViewModel()
        {
            // Convertimos el diccionario en lista observable para enlazar en la UI
            ConsejosPlanSeguro = new ObservableCollection<Globals.ConsejoInfo>(
                Globals.planSeguroConsejos.Values
            );
        }

        public async Task CargarAsync()
        {
            try
            {
                var saved = await SecureStorage.GetAsync("plan_seguro_data");
                if (!string.IsNullOrWhiteSpace(saved))
                {
                    var loaded = JsonSerializer.Deserialize<Dictionary<string, Globals.ConsejoInfo>>(saved);
                    if (loaded != null)
                        Globals.planSeguroConsejos = loaded;
                }

                ConsejosPlanSeguro = new ObservableCollection<Globals.ConsejoInfo>(Globals.planSeguroConsejos.Values);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cargando Plan Seguro: {ex.Message}");
                ConsejosPlanSeguro = new ObservableCollection<Globals.ConsejoInfo>(Globals.planSeguroConsejos.Values);

            }
        }

        public async Task InicializarConsejosPlanSeguroAsync()
        {
            try
            {
                // Revisar si ya hay datos guardados
                var existing = await SecureStorage.GetAsync("plan_seguro_data");

                if (string.IsNullOrWhiteSpace(existing))
                {
                    // No hay datos guardados → guardar los valores por defecto
                    var json = JsonSerializer.Serialize(Globals.planSeguroConsejos);
                    await SecureStorage.SetAsync("plan_seguro_data", json);

                    Console.WriteLine("SecureStorage inicializado con consejos por defecto.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inicializando SecureStorage: {ex.Message}");
            }
        }

    }
}
