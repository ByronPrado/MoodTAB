using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoodTAB.Models;
using MoodTAB.Vistas;
using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MoodTAB.Services;
using System.Linq;
using System.Text.Json;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Platform;


namespace MoodTAB.ViewModel

{

    public partial class DiarioViewModel : ObservableObject
    {
        [ObservableProperty] ObservableCollection<Diario> diarios = new();

        // Pregutas de Rutina
        [ObservableProperty] string pregunta1 = "";
        [ObservableProperty] string pregunta2 = "";
        [ObservableProperty] string pregunta3 = "";

        [ObservableProperty] string idpsy = Globals.id_psiquiatra_DB;

        // Horas registradas
        [ObservableProperty] double horasCelular;
        [ObservableProperty] double horasRedes;

        // Sliders
        [ObservableProperty] int animo = 0;
        [ObservableProperty] int apetito = 0;
        [ObservableProperty] int energia = 0;
        [ObservableProperty] int calidadSueno = 0;
        [ObservableProperty] int bateriaSocial = 0;
        [ObservableProperty] string sliders = "";


        // SmartWatch
        [ObservableProperty] public int ritmoCardiaco;
        [ObservableProperty] public int variabilidadFrecuenciaCardiaca;

        // Pasos
        [ObservableProperty] int cantidadPasos;

        // Calidad Sueño
        [ObservableProperty] string horasSueno ="0";
        [ObservableProperty] public TimeSpan horaDurmio;
        [ObservableProperty] public TimeSpan horaDesperto;

        // Switches: true = manual, false = smartwatch
        [ObservableProperty] private bool healthDataManual = Globals.OptionManual;
        

        // Otros
        [ObservableProperty] string error;
        [ObservableProperty] bool optionSuenoTrue = Globals.OptionSueno;
        [ObservableProperty] bool optionSuenoFalse = !Globals.OptionSueno;
        [ObservableProperty] bool optionHR = Globals.OptionHR;
        [ObservableProperty] bool optionHRV = Globals.OptionHVR;
        [ObservableProperty] bool optionPasos = Globals.OptionPasos;
        [ObservableProperty] bool actividadDiaria = Globals.OptionHR || Globals.OptionHVR || Globals.OptionPasos;
        public List<string> redes =
    [
        "com.whatsapp",                 //whatsapp
        "com.instagram.android",        //instagram
        "com.facebook.katana",          //facebook
        "com.discord",                  //discord
        "com.zhiliaoapp.musically",     //tiktok
        "com.pinterest",                //pinterest
        "com.tumblr"                    //tumblr
    ];
        public long redesociales = 0;
        public long horast = 0;

        // Servicios
        private readonly IStepCounterService stepService;
        private readonly IDictationService dictationService;
        private readonly IHealthDataService healthDataService;

        public DiarioViewModel(IStepCounterService stepService, IDictationService dictationService)
        {
            this.stepService = stepService;
            this.dictationService = dictationService;
            #if ANDROID
                Console.WriteLine("🔥 DiarioViewModel CONSTRUCTOR EJECUTADO");

                this.healthDataService = new HealthDataService();
                // Levantar servicio automáticamente
                _ = InitializeHealthDataAsync();
            #endif

            stepService.Start();

            HorasCelular = 0;
            HorasRedes = 0;
            CantidadPasos = 0;
            Error = "";
            VariabilidadFrecuenciaCardiaca = 0;
            HoraDurmio = TimeSpan.Zero;
            HoraDesperto = TimeSpan.Zero;

            _ = LoadDiariosAsync();
        }

        // Ajuste de valores sueño (24 horas)
        partial void OnHorasSuenoChanged(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (double.TryParse(value, 
                                    System.Globalization.NumberStyles.Any, 
                                    System.Globalization.CultureInfo.InvariantCulture, 
                                    out double horas))
                {
                    // Redondear al entero más cercano
                    int horasInt = (int)Math.Round(horas);

                    // Limitar a 24 horas máximo
                    if (horasInt > 24)
                        horasInt = 24;

                    HorasSueno = horasInt.ToString();
                }
                else
                {
                    HorasSueno = "0"; // fallback si no se puede parsear
                }
            }
        }


        #if ANDROID
            private async Task InitializeHealthDataAsync()
            {
                try
                {
                    Console.WriteLine("InitializeHealthDataAsync llamado");

                    // Solicitar permisos
                    bool permissionsGranted = await healthDataService.InitializeAndRequestPermissionsAsync();
                    Console.WriteLine($"Permisos concedidos? {permissionsGranted}");
                    if (!permissionsGranted)
                    {
                        Error = "Permisos de HealthConnect no concedidos";
                        return;
                    }

                    // Cargar todos los datos
                    await healthDataService.LoadAllHealthDataAsync();

                    // Asignar valores al viewmodel
                    CantidadPasos = healthDataService.TotalSteps;
                    RitmoCardiaco = healthDataService.HeartRate;
                    VariabilidadFrecuenciaCardiaca = healthDataService.HRV;
                    HorasSueno = healthDataService.SleepHours.ToString();
                    // Si quieres distancia también
                    // DistanciaKm = healthDataService.DistanceKm;
                    Console.WriteLine($"valores en viewmodel:pasos{CantidadPasos},ritmocardiaco {RitmoCardiaco},HRV {VariabilidadFrecuenciaCardiaca},HorasSueno {HorasSueno}");
                }
                catch (Exception ex)
                {
                    Error = ex.Message;
                }
            }
        #endif

        // Cargar y guardar
        private async Task LoadDiariosAsync()
        {
            var items = await App.Database.GetDiarioAsync();
            Diarios = new ObservableCollection<Diario>(items);
#if ANDROID
            var stats = UsageStatsHelper.GetAppUsageStats() ?? new Dictionary<string, long>();
            redesociales = 0;
            horast = 0;
            foreach (var stat in stats.OrderByDescending(x => x.Value).Take(20)) // Top 20 apps
            {
                var appName = stat.Key;
                var timeMinutes = stat.Value / 60000;
                if (redes.Contains(appName))
                {
                    redesociales += timeMinutes;
                }
                horast += timeMinutes;
            }

            HorasRedes = redesociales / 60.0;
            HorasCelular = horast / 60.0;

#endif
        }

        [RelayCommand]
        private async Task GuardarDiario()
        {
            try
            {
                var main = Application.Current?.MainPage;
                if (main == null) return;

                if (string.IsNullOrWhiteSpace(Pregunta1))
                {
                    await main.DisplayAlert("Campos en blanco", "No se puede dejar los campos en blanco", "OK");
                    return;
                }

                // Mostrar confirmación antes de enviar
                bool confirmacion = await main.DisplayAlert(
                    "Confirmar envío",
                    "¿Estás seguro de que quieres enviar el diario emocional?",
                    "Sí", "Cancelar"
                );

                if (!confirmacion)
                    return; // el usuario canceló
                //horas_dormidas
                if (HoraDesperto <= HoraDurmio)
                {
                    HoraDesperto = HoraDesperto.Add(new TimeSpan(1, 0, 0, 0)); // +1 día
                }

                var aux_horas_sueno = HoraDesperto - HoraDurmio;
                int integer_horas = aux_horas_sueno.Hours;
                System.Diagnostics.Debug.WriteLine($"Horas de sueño: {aux_horas_sueno.TotalHours}; integer_horas:{integer_horas}");
                
                //Sliders
                Sliders = Animo.ToString() + "," + Apetito.ToString() + "," + Energia.ToString() + "," + BateriaSocial.ToString() + "," + CalidadSueno.ToString();
                var diario = new Diario
                {
                    Emocion_Diaria = Sliders,
                    Descripcion = Pregunta1+"/"+Pregunta2+"/"+Pregunta3,
                    Horas_Celular = HorasCelular,
                    Horas_Redes = HorasRedes,
                    Horas_Sueno = integer_horas.ToString(),
                    HR_RitmoCardiaco = RitmoCardiaco,
                    HRV_VariabilidadFrecuencia = VariabilidadFrecuenciaCardiaca,
                    Hora_Durmio = HoraDurmio,
                    Hora_Desperto = HoraDesperto,
                    Cantidad_Pasos = CantidadPasos,
                    CreatedAt = DateTime.UtcNow,
                };

                await App.Database.SaveDiarioAsync(diario);
                await LoadDiariosAsync();

                var payload_sueno = 0;
                if (Globals.OptionSueno)
                {
                    int.TryParse(HorasSueno, out payload_sueno);
                }
                else
                {
                    payload_sueno = integer_horas;
                }
                var payload = new
                {
                    ID_Paciente = Globals.id_paciente_DB,
                    Emociones = Sliders,
                    Descripcion = Pregunta1+"/"+Pregunta2+"/"+Pregunta3,
                    Pasos = CantidadPasos,
                    Horas_celular = (int)HorasCelular,
                    Horas_redes = (int)HorasRedes,
                    Hora_dormida = payload_sueno,
                    HR_RitmoCardiaco = RitmoCardiaco,
                    HRV_VariabilidadFrecuencia = VariabilidadFrecuenciaCardiaca,
                    Hora_durmio = HoraDurmio.ToString(),
                    Hora_desperto = HoraDesperto.ToString(),
                    Fecha = DateTime.UtcNow
                };

                var url = $"{Globals.direccion_ngrok}api/DiarioEmocional";
                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                using var client = new HttpClient();
                var response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    await main.DisplayAlert("¡Listo!", "Diario enviado correctamente.", "OK");

                     try
                    {
                        var diarioAnterior = await App.Database.GetDiarioAnteriorAsync(diario.CreatedAt);

                        string anteriorResumen = diarioAnterior != null
                            ? $"Emociones: {diarioAnterior.Emocion_Diaria}, Descripción: {diarioAnterior.Descripcion}, Fecha: {diarioAnterior.CreatedAt}"
                            : null;


                        // Crear payload del log
                        var logPayload = new
                        {
                            ID_Paciente = Globals.id_paciente_DB,
                            ID_Psiquiatra = Globals.id_psiquiatra_DB ?? "1",
                            TipoLog = "DiarioEmocional",
                            Actual = $"Emociones: {Sliders}, Descripción: {Pregunta1}/{Pregunta2}/{Pregunta3}",
                            Anterior = anteriorResumen,
                            Fecha = DateTime.UtcNow
                        };

                        var logUrl = $"{Globals.direccion_ngrok}api/Logs";
                        var logJson = JsonSerializer.Serialize(logPayload);
                        var logContent = new StringContent(logJson, System.Text.Encoding.UTF8, "application/json");

                        using var logClient = new HttpClient();
                        var logResponse = await logClient.PostAsync(logUrl, logContent);

                        if (!logResponse.IsSuccessStatusCode)
                        {
                            var logError = await logResponse.Content.ReadAsStringAsync();
                            System.Diagnostics.Debug.WriteLine($"Error al subir el log: {logError}");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Log subido correctamente.");
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error al enviar el log: {ex.Message}");
                    }
                }
                else
                {
                    var errorMsg = await response.Content.ReadAsStringAsync();
                    await main.DisplayAlert("Error", $"No se pudo enviar el diario.\n{errorMsg}", "OK");
                }
            }
            catch (Exception e)
            {
                Error = e.Message;
                Pregunta1 = Error;
            }
        }


        // Debug
        [RelayCommand]
        private async Task BorrarDiario()
        {
            try
            {
                var listadiarios = Diarios.ToList();
                foreach (Diario diario in listadiarios)
                {
                    await App.Database.DeleteDiarioAsync(diario);
                }
                await LoadDiariosAsync();
            }
            catch (Exception e)
            {
                Error = e.Message;
            }
        }


        // Dictar preguntas
        [RelayCommand]
        public async Task DictarDiario1()
        {
            Pregunta1 = await dictationService.StartDictationAsync();
        }
        [RelayCommand]
        public async Task DictarDiario2()
        {
            Pregunta2 = await dictationService.StartDictationAsync();
        }
        [RelayCommand]
        public async Task DictarDiario3()
        {
            Pregunta3 = await dictationService.StartDictationAsync();
        }

        //popup
        public bool DebeMostrarTutorial =>
        !Preferences.Get("tutorialdiario_shown", false);
        }
}