using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel; // para MainThread
using System;

namespace MoodTAB.ViewModel
{
    public partial class EditarPlanSeguroViewModel : ObservableObject
    {
        // ===== LISTAS DEL PLAN =====
        [ObservableProperty] private ObservableCollection<string> senalesAlerta = new();
        [ObservableProperty] private ObservableCollection<string> estrategiasInternas = new();
        [ObservableProperty] private ObservableCollection<string> ambienteSeguro = new();

        [ObservableProperty] private ObservableCollection<PersonaContacto> personasDistraerme = new();
        [ObservableProperty] private ObservableCollection<PersonaContacto> personasPedirAyuda = new();
        [ObservableProperty] private ObservableCollection<PersonaContacto> profesionales = new();

        // ===== CAMPOS TEMPORALES PARA AGREGAR ITEM =====
        [ObservableProperty] private string nuevoItemTexto;
        [ObservableProperty] private string nuevoNombreContacto;
        [ObservableProperty] private string nuevoFonoContacto;

        // ===== NÚMERO DE EMERGENCIA =====
        [ObservableProperty]
        private string numeroEmergencia;

        public EditarPlanSeguroViewModel()
        {
            // Cargar datos previos (no bloqueante)
            _ = LoadAsync();
        }

        // Intenta cargar desde SecureStorage; soporta estructuras con/ sin numeroEmergencia.
        private async Task LoadAsync()
        {
            try
            {
                var json = await SecureStorage.GetAsync("plan_seguro_data");
                if (string.IsNullOrWhiteSpace(json))
                    return;

                // Intentamos parsear generically para soportar varios formatos
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                // Cargar listas si existen
                if (root.TryGetProperty("SenalesAlerta", out var sA))
                {
                    var list = new ObservableCollection<string>();
                    foreach (var el in sA.EnumerateArray())
                        list.Add(el.GetString() ?? string.Empty);

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        SenalesAlerta = list;
                    });
                }

                if (root.TryGetProperty("EstrategiasInternas", out var eI))
                {
                    var list = new ObservableCollection<string>();
                    foreach (var el in eI.EnumerateArray())
                        list.Add(el.GetString() ?? string.Empty);

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        EstrategiasInternas = list;
                    });
                }

                if (root.TryGetProperty("AmbienteSeguro", out var aS))
                {
                    var list = new ObservableCollection<string>();
                    foreach (var el in aS.EnumerateArray())
                        list.Add(el.GetString() ?? string.Empty);

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        AmbienteSeguro = list;
                    });
                }

                // Función auxiliar para cargar contactos
                void LoadContactList(string propertyName, Action<ObservableCollection<PersonaContacto>> setAction)
                {
                    if (!root.TryGetProperty(propertyName, out var arr)) return;
                    var list = new ObservableCollection<PersonaContacto>();
                    foreach (var el in arr.EnumerateArray())
                    {
                        var nombre = el.TryGetProperty("Nombre", out var n) ? n.GetString() ?? string.Empty : string.Empty;
                        var fono = el.TryGetProperty("Fono", out var f) ? f.GetString() ?? string.Empty : string.Empty;
                        list.Add(new PersonaContacto { Nombre = nombre, Fono = fono });
                    }
                    MainThread.BeginInvokeOnMainThread(() => setAction(list));
                }

                LoadContactList("PersonasDistraerme", c => PersonasDistraerme = c);
                LoadContactList("PersonasPedirAyuda", c => PersonasPedirAyuda = c);
                LoadContactList("Profesionales", c => Profesionales = c);

                // Si viene numero de emergencia guardado
                if (root.TryGetProperty("NumeroEmergencia", out var num))
                {
                    var numStr = num.GetString() ?? string.Empty;
                    MainThread.BeginInvokeOnMainThread(() => NumeroEmergencia = numStr);
                }
                else
                {
                    // Para compatibilidad con formatos anteriores: busca en la raíz (opcional)
                    if (root.TryGetProperty("Numero", out var num2))
                    {
                        MainThread.BeginInvokeOnMainThread(() => NumeroEmergencia = num2.GetString() ?? string.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                // No romper la app por un JSON corrupto; solo logueamos
                System.Diagnostics.Debug.WriteLine($"LoadAsync PlanSeguro: {ex.Message}");
            }
        }

        // ===== FUNCIONES PARA AGREGAR DATOS A CADA LISTA =====
        [RelayCommand]
        private void AgregarSenalAlerta()
        {
            if (!string.IsNullOrWhiteSpace(NuevoItemTexto))
            {
                SenalesAlerta.Add(NuevoItemTexto.Trim());
                NuevoItemTexto = string.Empty;
            }
        }

        [RelayCommand]
        private void AgregarEstrategiaInterna()
        {
            if (!string.IsNullOrWhiteSpace(NuevoItemTexto))
            {
                EstrategiasInternas.Add(NuevoItemTexto.Trim());
                NuevoItemTexto = string.Empty;
            }
        }

        [RelayCommand]
        private void AgregarAmbienteSeguro()
        {
            if (!string.IsNullOrWhiteSpace(NuevoItemTexto))
            {
                AmbienteSeguro.Add(NuevoItemTexto.Trim());
                NuevoItemTexto = string.Empty;
            }
        }

        [RelayCommand]
        private void AgregarPersonaDistraerme()
        {
            if (!string.IsNullOrWhiteSpace(NuevoNombreContacto) && !string.IsNullOrWhiteSpace(NuevoFonoContacto))
            {
                PersonasDistraerme.Add(new PersonaContacto
                {
                    Nombre = NuevoNombreContacto.Trim(),
                    Fono = NuevoFonoContacto.Trim()
                });

                NuevoNombreContacto = string.Empty;
                NuevoFonoContacto = string.Empty;
            }
        }

        [RelayCommand]
        private void AgregarPersonaPedirAyuda()
        {
            if (!string.IsNullOrWhiteSpace(NuevoNombreContacto) && !string.IsNullOrWhiteSpace(NuevoFonoContacto))
            {
                PersonasPedirAyuda.Add(new PersonaContacto
                {
                    Nombre = NuevoNombreContacto.Trim(),
                    Fono = NuevoFonoContacto.Trim()
                });

                NuevoNombreContacto = string.Empty;
                NuevoFonoContacto = string.Empty;
            }
        }

        [RelayCommand]
        private void AgregarProfesional()
        {
            if (!string.IsNullOrWhiteSpace(NuevoNombreContacto) && !string.IsNullOrWhiteSpace(NuevoFonoContacto))
            {
                Profesionales.Add(new PersonaContacto
                {
                    Nombre = NuevoNombreContacto.Trim(),
                    Fono = NuevoFonoContacto.Trim()
                });

                NuevoNombreContacto = string.Empty;
                NuevoFonoContacto = string.Empty;
            }
        }

        // ===== COMANDO ELIMINAR (simple) =====
        [RelayCommand]
        void EliminarSenalAlerta(string item) => SenalesAlerta.Remove(item);

        [RelayCommand]
        void EliminarEstrategiaInterna(string item) => EstrategiasInternas.Remove(item);

        [RelayCommand]
        void EliminarAmbienteSeguro(string item) => AmbienteSeguro.Remove(item);

        [RelayCommand]
        void EliminarPersonaDistraerme(PersonaContacto persona) => PersonasDistraerme.Remove(persona);

        [RelayCommand]
        void EliminarPersonaPedirAyuda(PersonaContacto persona) => PersonasPedirAyuda.Remove(persona);

        [RelayCommand]
        void EliminarProfesional(PersonaContacto persona) => Profesionales.Remove(persona);

        // ===== GUARDAR =====
        [RelayCommand]
        private async Task GuardarAsync()
        {
            try
            {
                // Serializamos un objeto que incluye las listas y el número de emergencia
                var data = new
                {
                    SenalesAlerta = SenalesAlerta.ToList(),
                    EstrategiasInternas = EstrategiasInternas.ToList(),
                    AmbienteSeguro = AmbienteSeguro.ToList(),
                    PersonasDistraerme = PersonasDistraerme.ToList(),
                    PersonasPedirAyuda = PersonasPedirAyuda.ToList(),
                    Profesionales = Profesionales.ToList(),
                    NumeroEmergencia = NumeroEmergencia ?? string.Empty
                };

                var json = JsonSerializer.Serialize(data);
                await SecureStorage.SetAsync("plan_seguro_data", json);

                GuardadoExitoso?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error guardando Plan Seguro: {ex.Message}");
            }
        }

        // ===== EVENTO PARA LA VISTA =====
        public event EventHandler GuardadoExitoso;
    }
}
