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

namespace MoodTAB.ViewModel

{

    public partial class DiarioViewModel : ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<Diario> diarios = new();

        [ObservableProperty]
        ObservableCollection<string> emocionDiaria = new();

        [ObservableProperty]
        ObservableCollection<EmocionItem> listaEmociones = new ObservableCollection<EmocionItem>();

        [ObservableProperty]
        string descDia;

        [ObservableProperty]
        double horasCelular;

        [ObservableProperty]
        double horasRedes;

        [ObservableProperty]
        double horasYT;

        [ObservableProperty]
        int cantidadPasos;

        [ObservableProperty]
        string horasSueno;

        [ObservableProperty]
        string error;

        [ObservableProperty]
        double intensidadEmocion = 0.0;
        [ObservableProperty]
        ObservableCollection<string> emocionesSeleccionadas = [];
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
        public long horasyutu = 0;
        private readonly IStepCounterService stepService;

        private readonly IDictationService dictationService;

        [ObservableProperty]
        string test;

        public DiarioViewModel(IStepCounterService stepService, IDictationService dictationService)
        {
            this.stepService = stepService;
            this.dictationService = dictationService;

            stepService.Start();

            DescDia = "";
            HorasCelular = 0;
            HorasRedes = 0;
            HorasYT = 0;
            CantidadPasos = (int)stepService.TotalSteps;
            HorasSueno = "0";
            Error = "";
            test = "veamos";

            foreach (var key in Globals.colores.Keys)
            {
                var item = new EmocionItem(
                    texto: key,
                    emoticon: Globals.emoticonos[key],
                    color: "White",
                    colorBorde: "gray"
                );
                listaEmociones.Add(item);

            }


            _ = LoadDiariosAsync();
        }

        public string UnirConComas(ObservableCollection<string> lista)
        {
            return string.Join(",", lista);
        }

        [RelayCommand]
        private void SeleccionarEmocion(string emocion)
        {
            var item = ListaEmociones.FirstOrDefault(e => e.Texto == emocion);
            if (item == null) return;
            var index = ListaEmociones.IndexOf(item);
            if (EmocionDiaria.Contains(emocion))
            {
                EmocionDiaria.Remove(emocion);
                item.Color = "White";
                item.ColorBorde = "Gray";
            }
            else
            {
                EmocionDiaria.Add(emocion);
                item.Color = Globals.colores[emocion];
                item.ColorBorde = Globals.bordes[emocion];
            }
            if (index >= 0)
            {
                ListaEmociones[index] = item;
            }
        }

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

        private async Task LoadDiariosAsync()
        {
            var items = await App.Database.GetDiarioAsync();
            Diarios = new ObservableCollection<Diario>(items);
#if ANDROID
            var stats = UsageStatsHelper.GetAppUsageStats() ?? new Dictionary<string, long>();
            redesociales = 0;
            horast = 0;
            horasyutu = 0;
            foreach (var stat in stats.OrderByDescending(x => x.Value).Take(20)) // Top 20 apps
            {
                var appName = stat.Key;
                var timeMinutes = stat.Value / 60000;
                if (redes.Contains(appName))
                {
                    redesociales += timeMinutes;
                }
                if (appName == "com.google.android.youtube")
                {
                    horasyutu += timeMinutes;
                }
                horast += timeMinutes;
            }

            HorasRedes = redesociales / 60.0;
            HorasCelular = horast / 60.0;
            HorasYT = horasyutu / 60.0;

#endif
        }

        [RelayCommand]
        private async Task GuardarDiario()
        {
            try
            {
                var main = Application.Current?.MainPage;
                if (main == null) return;
                if (EmocionDiaria.Count == 0 || string.IsNullOrWhiteSpace(DescDia))
                {
                    await main.DisplayAlert("Campos en blanco", "No se puede dejar los campos en blanco", "OK");
                    return;
                }
                var diario = new Diario
                {
                    Emocion_Diaria = UnirConComas(EmocionDiaria),
                    Descripcion = DescDia,
                    Horas_Celular = HorasCelular,
                    Horas_Redes = HorasRedes,
                    Horas_Yt = HorasYT,
                    Horas_Sueno = HorasSueno,
                    Cantidad_Pasos = CantidadPasos,
                    CreatedAt = DateTime.UtcNow,

                };

                await App.Database.SaveDiarioAsync(diario);
                await LoadDiariosAsync();

                var payload = new
                {
                    ID_Paciente = Globals.id_paciente_DB, // Usa el id del paciente logueado
                    Emociones = JsonSerializer.Serialize(
                        EmocionDiaria.ToDictionary(e => e, e => 1) // Puedes ajustar el valor según intensidad si lo tienes
                    ),
                    Descripcion = DescDia,
                    Pasos = CantidadPasos,
                    Horas_celular = (int)HorasCelular,
                    Horas_redes = (int)HorasRedes,
                    Horas_Yt = (int)HorasYT,
                    Hora_dormida = HorasSueno,
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
                DescDia = Error;
            }

        }

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
        
        [RelayCommand]
        public async Task DictarDiario()
        {
            DescDia = await dictationService.StartDictationAsync();
        }
    }
}