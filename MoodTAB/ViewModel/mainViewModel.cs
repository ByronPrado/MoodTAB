using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoodTAB.Models;
using MoodTAB.Vistas;
using MoodTAB.Services;
using System.Text.Json;
using Microsoft.Maui.ApplicationModel.Communication;
using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Net.Http;

using Microsoft.Maui.Controls;


namespace MoodTAB.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly INotificationManagerService notificationManager;
        //propiedades del usuario
        [ObservableProperty]
        private string _title = "Bienvenido a MoodTAB";
        private string _nameUser = "usuario";
        private string _emailUsuario = "mail";

        //para comprobar is tiene cuestionarios dipsonibles
        [ObservableProperty]
        private bool cuestionario = false;
        [ObservableProperty]
        private bool isBusy;
        //labels text bindings
        [ObservableProperty]
        private string _titleApi = "Bienvenido a MoodTAB";
        [ObservableProperty]
        private string _textoActividadCuestionario = "Revisando...";


        [ObservableProperty]
        private string _textoTituloConsejo = "TituloConsejo";
        [ObservableProperty]
        private string _textoContenidoConsejo = "Contenido consejo";
        [ObservableProperty]
        private bool _mostrarConsejo = true;


        private string _consejoKey = string.Empty;
        const string ConsejosOverridesKey = "consejos_overrides_v1";
        private Dictionary<string, bool>? _overridesCached; // cache en memoria
        public string NameUser
        {
            get => _nameUser;
            set => SetProperty(ref _nameUser, value);
        }
        public string EmailUsuario
        {
            get => _emailUsuario;
            set => SetProperty(ref _emailUsuario, value);
        }
    
        //constructor
        public MainViewModel(INotificationManagerService notificationManager)
        {
            this.notificationManager = notificationManager;
            ActualizarDatosUsuario();
            try
            {
                CargarSaludoAsync();
                getCuestionario();
            }
            catch (Exception e)
            {
                TitleApi = "No se pudo conectar a la web " + e.Message;
            }

            var storedValue = SecureStorage.GetAsync("healthdata_manual").Result ?? "false";
            Globals.OptionManual = bool.Parse(storedValue);
            storedValue = SecureStorage.GetAsync("ingreso_sueno").Result ?? "false";
            Globals.OptionSueno = bool.Parse(storedValue);
            storedValue = SecureStorage.GetAsync("ingreso_hr").Result ?? "false";
            Globals.OptionHR = bool.Parse(storedValue);
            storedValue = SecureStorage.GetAsync("ingreso_hrv").Result ?? "false";
            Globals.OptionHVR = bool.Parse(storedValue);
            storedValue = SecureStorage.GetAsync("ingreso_pasos").Result ?? "false";
            Globals.OptionPasos = bool.Parse(storedValue);
            storedValue = SecureStorage.GetAsync("mostrar_comentarios").Result ?? "true";
            Globals.OpcionMostrarConsejos = bool.Parse(storedValue);
            MostrarConsejo = Globals.OpcionMostrarConsejos;
            // iniciar carga del cache de overrides en background y luego cargar el consejo
            _ = InitializeOverridesAndLoadConsejoAsync();

        }

        //funciones
        public void ActualizarDatosUsuario()
        {
            // Inicializar el nombre de usuario
            NameUser = SecureStorage.GetAsync("user_nombre").Result ?? "TestActDatosusuario";
            EmailUsuario = SecureStorage.GetAsync("user_email").Result ?? "test";
            try { Title = $"Hola {NameUser}"; }
            catch (Exception ex)
            {
                Console.WriteLine($"[DEBUG]ERROR mainviewmodel: {ex.Message}");
            }
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
                Console.WriteLine($"[DEBUG]ERROR mainviewmodel: {ex.Message}");
            }
        }

        public async Task<string> ObtenerSaludoAsync()
        {
            try
            {
                using var client = new HttpClient
                {
                    Timeout = TimeSpan.FromSeconds(10)
                };
                var url = $"{Globals.direccion_ngrok}api/pacientes";
                return await client.GetStringAsync(url);
            }
            catch (TaskCanceledException ex)
            {
                // Esto ocurre si se supera el Timeout
                TitleApi = "Error: La petición al servidor tardó demasiado (timeout).";
                Console.WriteLine($"Timeout: {ex.Message}");
                return "Error: Timeout";
            }
            catch (HttpRequestException ex)
            {
                TitleApi = "Error de conexión con el servidor.";
                Console.WriteLine($"Http error: {ex.Message}");
                return "Error: Conexión fallida";

            }
            catch (Exception ex)
            {
                TitleApi = $"Error inesperado: {ex.Message}";
                Console.WriteLine($"Excepción inesperada: {ex}");
                return $"Error inesperado: {ex.Message}";
            }

        }

        public INavigation? Navigation { get; set; }

        [RelayCommand]
        private async Task MovetoPage(string pageName)
        {
            try
            {
                IsBusy = true;
                ActualizarDatosUsuario();
                Page? page = pageName switch
                {
                    "CuestionarioPage" => new CuestionarioPage(),
                    "TestPage" => new TestPage(),
                    "DiarioPage" => new DiarioPage(),
                    "CalendarioPage" => new CalendarioDiario(),
                    "UserPage" => new UserPage(),
                    "PlanSeguroPage" => new PlanSeguroPage(),
                    "PastilleroPage" => new PastilleroPage(),
                    "Healthconn" => new HealthDataPage(),
                    _ => null
                };

                if (page != null && Navigation != null)
                    await Navigation.PushAsync(page);
                else
                    Title = $"Error: Página no encontrada o Navigation es null {pageName}";
            }
            catch (Exception ex)
            {
                Title = $"Error al navegar: {ex.Message}";
                Console.WriteLine($"[DEBUG]ERROR Inavigation: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
        public async void getCuestionario()
        {
            try
            {
                var notif = SecureStorage.GetAsync("notif_c").Result;
                var url = $"{Globals.direccion_ngrok}api/formulario/{Globals.id_paciente_DB}";
                using var client = new HttpClient()
                {
                    Timeout = TimeSpan.FromSeconds(10)
                };
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    Globals.cuestionario_pendiente = false;
                    Globals.cuestionario = "{\"status\":404,\"title\":\"Not Found\"}";
                    Cuestionario = false;
                    TitleApi = "Sin Cuestionarios Pendientes.";
                    return;
                }

                var content = await response.Content.ReadAsStringAsync();
                Globals.cuestionario = content;
                // ✅ Analizar el JSON antes de deserializar
                using var doc = JsonDocument.Parse(content);
                var root = doc.RootElement;

                if (root.ValueKind != JsonValueKind.Array)
                {
                    TitleApi = "El JSON recibido no es una lista de cuestionarios.";
                    Globals.cuestionario_pendiente = false;
                    Cuestionario = false;
                    return;
                }

                int cantidadCuestionarios = root.GetArrayLength();
                if (cantidadCuestionarios == 1)
                {
                    TextoActividadCuestionario = "Responder a la brevedad";

                }
                else
                {
                    TextoActividadCuestionario = "Tienes " + cantidadCuestionarios.ToString() + " cuestionarios disponibles";
                }


                try
                {
                    var cuestionarios = JsonSerializer.Deserialize<List<CuestionarioData>>(content);

                    if (cuestionarios != null && cuestionarios.Any())
                    {
                        Globals.cuestionario_pendiente = true;
                        Cuestionario = true;

                        if (notif == null)
                        {
                            notificationManager.SendNotification(
                                "Cuestionario MoodTAB",
                                "Tienes un cuestionario pendiente por responder.",
                                DateTime.Today.AddMinutes(1),
                                1001
                            );
                            await SecureStorage.SetAsync("notif_c", "active");
                        }
                    }
                    else
                    {
                        // Lista vacía -> no hay cuestionarios
                        Globals.cuestionario_pendiente = false;
                        Cuestionario = false;
                    }
                }
                catch (Exception ex)
                {
                    // Si no se puede parsear, tratamos como "sin cuestionarios"
                    Globals.cuestionario_pendiente = false;
                    Cuestionario = false;
                    Console.WriteLine($"Error parseando cuestionarios: {ex.Message}");
                }
            }
            catch (TaskCanceledException ex)
            {
                // Esto ocurre si se supera el Timeout
                TitleApi = "Error: La petición al servidor tardó demasiado (timeout).";
                Console.WriteLine($"Timeout: {ex.Message}");
            }
            catch (HttpRequestException ex)
            {
                TitleApi = "Error de conexión con el servidor.";
                Console.WriteLine($"Http error: {ex.Message}");
            }
            catch (Exception ex)
            {
                TitleApi = $"Error inesperado: {ex.Message}";
                Console.WriteLine($"Excepción inesperada: {ex}");
            }
        }

        // Carga un consejo aleatorio, priorizando los que evalúan Util==true y aplicando overrides desde el cache
        public async Task CargarConsejoAsync()
        {
            try
            {
                var overrides = _overridesCached; // usar cache en memoria (rápido)

                // construir lista efectiva con flag util final (override > definido)
                var effective = Globals.consejos
                    .Select(kv =>
                    {
                        bool util = kv.Value.Util;
                        if (overrides != null && overrides.TryGetValue(kv.Key, out var o)) util = o;
                        return new { Key = kv.Key, Consejo = kv.Value, Util = util };
                    })
                    .ToList();

                var preferred = effective.Where(e => e.Util).ToList();
                var pool = preferred.Any() ? preferred : effective;

                if (!pool.Any())
                {
                    _consejoKey = string.Empty;
                    TextoTituloConsejo = "Sin consejos";
                    TextoContenidoConsejo = "";
                    return;
                }

                var rnd = new Random();
                var pick = pool[rnd.Next(pool.Count)];

                _consejoKey = pick.Key;

                // actualizar propiedades en hilo UI
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    TextoTituloConsejo = pick.Consejo.Titulo;
                    TextoContenidoConsejo = pick.Consejo.Contenido;
                });
            }
            catch (Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    TextoTituloConsejo = "Error cargando consejo";
                    TextoContenidoConsejo = ex.Message;
                });
            }
        }

        // Marca el consejo actual como útil/no útil, actualiza cache e inicia persistencia en background; no bloquear UI
        public async Task MarcarConsejoUtilAsync(bool util)
        {
            if (string.IsNullOrEmpty(_consejoKey)) return;

            try
            {
                // actualizar cache en memoria inmediatamente
                if (_overridesCached == null) _overridesCached = new Dictionary<string, bool>();
                _overridesCached[_consejoKey] = util;

                // actualizar in-memory global para reflejar en comportamiento inmediato
                if (Globals.consejos.ContainsKey(_consejoKey))
                    Globals.consejos[_consejoKey].Util = util;

                // persistir en background (no await para no bloquear UI)
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await SaveOverridesAsync(_overridesCached).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        // loguear pero no romper la experiencia
                        Console.WriteLine($"Error persisting overrides: {ex.Message}");
                    }
                });

                // elegir y mostrar el siguiente consejo inmediatamente (usa cache)
                await CargarConsejoAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error guardando preferencia consejo: {ex.Message}");
            }
        } 
        async Task<Dictionary<string, bool>?> LoadOverridesAsync()
        {
            try
            {
                var s = await SecureStorage.GetAsync(ConsejosOverridesKey).ConfigureAwait(false);
                if (string.IsNullOrWhiteSpace(s)) return null;
                return JsonSerializer.Deserialize<Dictionary<string, bool>>(s);
            }
            catch
            {
                return null;
            }
        }

        async Task SaveOverridesAsync(Dictionary<string, bool> overrides)
        {
            try
            {
                var json = JsonSerializer.Serialize(overrides);
                await SecureStorage.SetAsync(ConsejosOverridesKey, json).ConfigureAwait(false);
            }
            catch
            {
                // fallar silenciosamente en dispositivos que no soporten SecureStorage
            }
        }
        // Inicializa cache de overrides y fuerza carga de consejo cuando esté listo
        private async Task InitializeOverridesAndLoadConsejoAsync()
        {
            try
            {
                _overridesCached = await LoadOverridesAsync().ConfigureAwait(false) ?? new Dictionary<string, bool>();
            }
            catch
            {
                _overridesCached = new Dictionary<string, bool>();
            }

            // cargar consejo usando el cache (asegura que UI se actualice)
            await CargarConsejoAsync().ConfigureAwait(false);
        }
    }
}