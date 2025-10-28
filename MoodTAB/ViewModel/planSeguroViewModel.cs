using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoodTAB;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace MoodTAB.ViewModel
{
    public partial class PlanSeguroViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<Globals.ConsejoInfo> consejosPlanSeguro;

        [ObservableProperty]
        private string numeroEmergencia;
        public PlanSeguroViewModel()
        {
            // Convertimos el diccionario en lista observable para enlazar en la UI
            ConsejosPlanSeguro = new ObservableCollection<Globals.ConsejoInfo>(
                Globals.planSeguroConsejos.Values
            );
            NumeroEmergencia = Globals.numeroEmergencia;
        }

        private class PlanSeguroData
        {
            public Dictionary<string, Globals.ConsejoInfo> Consejos { get; set; }
            public string NumeroEmergencia { get; set; }
        }
        public async Task CargarAsync()
        {
            try
            {
                var saved = await SecureStorage.GetAsync("plan_seguro_data");
                if (!string.IsNullOrWhiteSpace(saved))
                {
                    var loaded = JsonSerializer.Deserialize<PlanSeguroData>(saved);

                    if (loaded != null)
                    {
                        if (loaded.Consejos != null && loaded.Consejos.Count > 0)
                            Globals.planSeguroConsejos = loaded.Consejos;

                        if (!string.IsNullOrWhiteSpace(loaded.NumeroEmergencia))
                            Globals.numeroEmergencia = loaded.NumeroEmergencia;
                    }
                }

                ConsejosPlanSeguro = new ObservableCollection<Globals.ConsejoInfo>(
                    (Globals.planSeguroConsejos ?? new Dictionary<string, Globals.ConsejoInfo>()).Values
                );
                NumeroEmergencia = Globals.numeroEmergencia ?? string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cargando Plan Seguro: {ex.Message}");
                ConsejosPlanSeguro = new ObservableCollection<Globals.ConsejoInfo>(
                    (Globals.planSeguroConsejos ?? new Dictionary<string, Globals.ConsejoInfo>()).Values
                );
                NumeroEmergencia = Globals.numeroEmergencia ?? string.Empty;
            }
        }
        public async Task InicializarConsejosPlanSeguroAsync()
        {
            try
            {
                var existing = await SecureStorage.GetAsync("plan_seguro_data");

                if (string.IsNullOrWhiteSpace(existing))
                {
                    var data = new PlanSeguroData
                    {
                        Consejos = Globals.planSeguroConsejos,
                        NumeroEmergencia = Globals.numeroEmergencia
                    };

                    var json = JsonSerializer.Serialize(data);
                    await SecureStorage.SetAsync("plan_seguro_data", json);

                    Console.WriteLine("SecureStorage inicializado con consejos por defecto.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inicializando SecureStorage: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task Llamar(string phoneNumber)
        {
            // Debug: muestra en consola de Visual Studio
            Console.WriteLine($"[DEBUG] Intentando llamar a: {phoneNumber}");

            try
            {
                // Validar número
                if (string.IsNullOrWhiteSpace(phoneNumber))
                {
                    Console.WriteLine("[DEBUG] Número vacío o nulo. No se puede continuar.");
                    return;
                }

                // Abrir marcador telefónico
                var telUrl = $"tel:{phoneNumber}";
                Console.WriteLine($"[DEBUG] Ejecutando Launcher.OpenAsync con: {telUrl}");

                await Launcher.OpenAsync(telUrl);

                Console.WriteLine("[DEBUG] Llamada ejecutada correctamente (o marcador abierto).");
            }
            catch (FeatureNotSupportedException fex)
            {
                Console.WriteLine($"[ERROR] Función no soportada en este dispositivo: {fex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error general al intentar llamar: {ex.Message}");
            }
        }
    }
}
