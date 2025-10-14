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
        [ObservableProperty] string horasSueno;
        [ObservableProperty] public TimeSpan horaDurmio;
        [ObservableProperty] public TimeSpan horaDesperto;

        // Switches: true = manual, false = smartwatch
        [ObservableProperty] private bool isManualRitmoCardiaco = true; // Nuevo, default manual
        [ObservableProperty] private bool isManualVariabilidad = true;
        [ObservableProperty] private bool isManualPasos = true;
        [ObservableProperty] private bool isManualHoraDurmio = true;
        [ObservableProperty] private bool isManualHoraDesperto = true;
        [ObservableProperty] private bool isManualHorasSueno = true;
        [ObservableProperty] private bool healthDataManual;

        // Otros
        [ObservableProperty] string error;
        [ObservableProperty] bool optionSuenoTrue = Globals.OptionSueno;
        [ObservableProperty] bool optionSuenoFalse = !Globals.OptionSueno;
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

        public DiarioViewModel(IStepCounterService stepService, IDictationService dictationService)
        {
            this.stepService = stepService;
            this.dictationService = dictationService;

            stepService.Start();

            HorasCelular = 0;
            HorasRedes = 0;
            CantidadPasos = (int)stepService.TotalSteps;
            HorasSueno = "0";
            Error = "";
            RitmoCardiaco = 0;
            VariabilidadFrecuenciaCardiaca = 0;
            HoraDurmio = TimeSpan.Zero;
            HoraDesperto = TimeSpan.Zero;

            // Cargar el valor almacenado de healthdata_manual
            var storedValue = SecureStorage.GetAsync("healthdata_manual").Result ?? "false";
            HealthDataManual = bool.Parse(storedValue);


            _ = LoadDiariosAsync();
        }

        // Ajuste de valores sueño (24 horas)
        partial void OnHorasSuenoChanged(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                HorasSueno = new string(value.Where(char.IsDigit).ToArray());
            }
            if (int.TryParse(value, out int intValue) && intValue > 24)
            {
                HorasSueno = "24";
            }
        }

        // Manejo de cambios en switches (usando partial void para setters)
        partial void OnIsManualRitmoCardiacoChanged(bool value)
        {
            HandleSwitchChange(value, nameof(RitmoCardiaco));
        }
        partial void OnIsManualVariabilidadChanged(bool value)
        {
            HandleSwitchChange(value, nameof(VariabilidadFrecuenciaCardiaca));
        }
        partial void OnIsManualPasosChanged(bool value)
        {
            HandleSwitchChange(value, nameof(CantidadPasos));
            if (value) CantidadPasos = (int)stepService.TotalSteps; // Si manual, carga actual, pero permite editar
        }
        partial void OnIsManualHoraDurmioChanged(bool value)
        {
            HandleSwitchChange(value, nameof(HoraDurmio));
        }
        partial void OnIsManualHoraDespertoChanged(bool value)
        {
            HandleSwitchChange(value, nameof(HoraDesperto));
        }
        partial void OnIsManualHorasSuenoChanged(bool value)
        {
            HandleSwitchChange(value, nameof(HorasSueno));
        }
        private async void HandleSwitchChange(bool isManual, string propertyName)
        {
            if (!isManual)
            {
                // Smartwatch seleccionado
                await Application.Current.MainPage.DisplayAlert("Característica Futura", "La integración con smartwatch no está disponible aún.", "OK");
                // Resetear valor a default (puedes personalizar)
                SetPropertyByName(propertyName, propertyName.Contains("Hora") ? TimeSpan.Zero : (object)0);
            }
        }
        private void SetPropertyByName(string propertyName, object value)
        {
            // Helper para setear propiedades dinámicamente
            switch (propertyName)
            {
                case nameof(RitmoCardiaco): RitmoCardiaco = (int)value; break;
                case nameof(VariabilidadFrecuenciaCardiaca): VariabilidadFrecuenciaCardiaca = (int)value; break;
                case nameof(CantidadPasos): CantidadPasos = (int)value; break;
                case nameof(HoraDurmio): HoraDurmio = (TimeSpan)value; break;
                case nameof(HoraDesperto): HoraDesperto = (TimeSpan)value; break;
                case nameof(HorasSueno): HorasSueno = value.ToString(); break;
            }
        }


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
                    Ritmo_Cardiaco = RitmoCardiaco,
                    Variabilidad_Frecuencia_Cardiaca = VariabilidadFrecuenciaCardiaca,
                    Hora_Durmio = HoraDurmio,
                    Hora_Desperto = HoraDesperto,
                    Cantidad_Pasos = CantidadPasos,
                    CreatedAt = DateTime.UtcNow,
                };

                await App.Database.SaveDiarioAsync(diario);
                await LoadDiariosAsync();

                var payload = new
                {
                    ID_Paciente = Globals.id_paciente_DB,
                    Emociones = Sliders,
                    Descripcion = Pregunta1+"/"+Pregunta2+"/"+Pregunta3,
                    Pasos = CantidadPasos,
                    Horas_celular = (int)HorasCelular,
                    Horas_redes = (int)HorasRedes,
                    Hora_dormida = integer_horas,
                    RitmoCardiaco,
                    VariabilidadFrecuenciaCardiaca,
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
    }
}