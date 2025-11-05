using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel; // Launcher
using System;

namespace MoodTAB.ViewModel
{
    public partial class PlanSeguroViewModel : ObservableObject
    {
        //listas de elementos del plan seguro
        [ObservableProperty]
        private ObservableCollection<string> senalesAlerta = new();

        [ObservableProperty]
        private ObservableCollection<string> estrategiasInternas = new();

        [ObservableProperty]
        private ObservableCollection<string> ambienteSeguro = new();

        [ObservableProperty]
        private ObservableCollection<PersonaContacto> personasDistraerme = new();

        [ObservableProperty]
        private ObservableCollection<PersonaContacto> personasPedirAyuda = new();

        [ObservableProperty]
        private ObservableCollection<PersonaContacto> profesionales = new();

        [ObservableProperty]
        private string numeroEmergencia;

        [ObservableProperty]
        private string logTxt;

        [ObservableProperty]
        private bool isBusy = false;

        public PlanSeguroViewModel()
        {
            _ = CargarAsync();
            // No bloqueante; la vista llamará a CargarAsync en OnAppearing.
        }

        private class PlanSeguroData
        {
            public string[] SenalesAlerta { get; set; }
            public string[] EstrategiasInternas { get; set; }
            public string[] AmbienteSeguro { get; set; }

            public PersonaContacto[] PersonasDistraerme { get; set; }
            public PersonaContacto[] PersonasPedirAyuda { get; set; }
            public PersonaContacto[] Profesionales { get; set; }

            public string NumeroEmergencia { get; set; }
        }

        public async Task CargarAsync()
        {
            try
            {
                var saved = await SecureStorage.GetAsync("plan_seguro_data");
                string json = saved;
                LogTxt = json;
                // Si no hay datos guardados, usamos el jsonEjemplo (útil para pruebas)
                if (string.IsNullOrWhiteSpace(json))
                {
                    json = @"{
                        ""SenalesAlerta"": [""Sentirme muy ansioso/a"", ""Ganas de llorar sin razón"", ""Pensamientos de que no valgo nada""],
                        ""EstrategiasInternas"": [""Respirar profundo 10 veces"", ""Escuchar música relajante"", ""Salir a caminar 15 minutos""],
                        ""PersonasDistraerme"": [
                            { ""Nombre"": ""Ana (Amiga)"", ""Fono"": ""+56 9 1111 2222"" },
                            { ""Nombre"": ""Mamá"", ""Fono"": ""+56 9 3333 4444"" }
                        ],
                        ""PersonasPedirAyuda"": [
                            { ""Nombre"": ""Dr. Ramírez"", ""Fono"": ""+56 9 5555 6666"" },
                            { ""Nombre"": ""Hermano (Juan)"", ""Fono"": ""+56 9 7777 8888"" }
                        ],
                        ""Profesionales"": [
                            { ""Nombre"": ""Salud Responde"", ""Fono"": ""600 360 7777"" },
                            { ""Nombre"": ""Emergencia Clínica"", ""Fono"": ""2 2999 0000"" }
                        ],
                        ""AmbienteSeguro"": [""Despejar mi velador"", ""Guardar objetos peligrosos bajo llave""],
                        ""NumeroEmergencia"": """"
                    }";
                    LogTxt = json;

                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var loaded = JsonSerializer.Deserialize<PlanSeguroData>(json, options);

                if (loaded != null)
                {
                    SenalesAlerta = new ObservableCollection<string>(loaded.SenalesAlerta ?? Array.Empty<string>());
                    EstrategiasInternas = new ObservableCollection<string>(loaded.EstrategiasInternas ?? Array.Empty<string>());
                    AmbienteSeguro = new ObservableCollection<string>(loaded.AmbienteSeguro ?? Array.Empty<string>());

                    PersonasDistraerme = new ObservableCollection<PersonaContacto>(loaded.PersonasDistraerme ?? Array.Empty<PersonaContacto>());
                    PersonasPedirAyuda = new ObservableCollection<PersonaContacto>(loaded.PersonasPedirAyuda ?? Array.Empty<PersonaContacto>());
                    Profesionales = new ObservableCollection<PersonaContacto>(loaded.Profesionales ?? Array.Empty<PersonaContacto>());

                    NumeroEmergencia = loaded.NumeroEmergencia ?? string.Empty;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cargando Plan Seguro: {ex.Message}");
                // No romper UI: dejar colecciones vacías si falla.
                SenalesAlerta ??= new ObservableCollection<string>();
                EstrategiasInternas ??= new ObservableCollection<string>();
                AmbienteSeguro ??= new ObservableCollection<string>();
                PersonasDistraerme ??= new ObservableCollection<PersonaContacto>();
                PersonasPedirAyuda ??= new ObservableCollection<PersonaContacto>();
                Profesionales ??= new ObservableCollection<PersonaContacto>();
                NumeroEmergencia ??= string.Empty;
            }
        }

        [RelayCommand]
        private async Task Llamar(string phoneNumber)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(phoneNumber))
                    return;

                var telUrl = $"tel:{phoneNumber}";
                await Launcher.OpenAsync(telUrl);
            }
            catch (FeatureNotSupportedException fex)
            {
                System.Diagnostics.Debug.WriteLine($"Llamar no soportado: {fex.Message}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error llamando: {ex.Message}");
            }
        }
    }

}
