using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoodTAB.Models;
using MoodTAB.Vistas;
using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MoodTAB.Services;
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

        [ObservableProperty]
        string colorFeliz;
        [ObservableProperty]
        string colorEmocionado;
        [ObservableProperty]
        string colorCansado;
        [ObservableProperty]
        string colorTriste;
        [ObservableProperty]
        string colorFrustrado;
        [ObservableProperty]
        string colorEnojado;
        [ObservableProperty]
        string colorNeutro;
        [ObservableProperty]
        string colorAngustiado;
        [ObservableProperty]
        string colorAnsioso;


        [ObservableProperty]
        string colorFeliz_borde;
        [ObservableProperty]
        string colorEmocionado_borde;
        [ObservableProperty]
        string colorCansado_borde;
        [ObservableProperty]
        string colorTriste_borde;
        [ObservableProperty]
        string colorFrustrado_borde;
        [ObservableProperty]
        string colorEnojado_borde;
        [ObservableProperty]
        string colorNeutro_borde;
        [ObservableProperty]
        string colorAngustiado_borde;
        [ObservableProperty]
        string colorAnsioso_borde;

        public DiarioViewModel(IStepCounterService stepService)
        {
            this.stepService = stepService;
            stepService.Start();

            DescDia = "";
            HorasCelular = 0;
            HorasRedes = 0;
            HorasYT = 0;
            CantidadPasos = (int)stepService.TotalSteps;
            HorasSueno = "0";
            Error = "";

            ColorFeliz = "#FEF9C3";
            ColorEmocionado = "#FFEDD5";
            ColorCansado = "#F3E8FF";
            ColorTriste = "#DBEAFE";
            ColorFrustrado = "#FEE2E2";
            ColorEnojado = "#FEE2E2";
            ColorNeutro = "#F3F4F6";
            ColorAngustiado = "#E0E7FF";
            ColorAnsioso = "#CCFBF1";

            ColorFeliz_borde = "#FEF4A3";
            ColorEmocionado_borde = "#FEDAB0";
            ColorCansado_borde = "#EDDDFF";
            ColorTriste_borde = "#BFDBFE";
            ColorFrustrado_borde = "#FECACA";
            ColorEnojado_borde = "#FED5D5";
            ColorNeutro_borde = "#EBEDF0";
            ColorAngustiado_borde = "#CCD6FE";
            ColorAnsioso_borde = "#99F6E4";


            _ = LoadDiariosAsync();
        }

        public string UnirConComas(ObservableCollection<string> lista)
        {
            return string.Join(",", lista);
        }

        [RelayCommand]
        private void SeleccionarEmocion(string emocion)
        {
            if (EmocionDiaria.Contains(emocion))
            {
                EmocionDiaria.Remove(emocion);
                if (emocion == "Feliz")
                {
                    ColorFeliz = "#FEF9C3";
                    ColorFeliz_borde = "#FEF4A3";

                }
                if (emocion == "Emocionado")
                {
                    ColorEmocionado = "#FFEDD5";
                    ColorEmocionado_borde = "#FEDAB0";
                }
                if (emocion == "Cansado")
                {
                    ColorCansado = "#F3E8FF";
                    ColorCansado_borde = "#EDDDFF";
                }
                if (emocion == "Triste")
                {
                    ColorTriste = "#DBEAFE";
                    ColorTriste_borde = "#BFDBFE";
                }
                if (emocion == "Frustrado")
                {
                    ColorFrustrado = "#FEE2E2";
                    ColorFrustrado_borde = "#FECACA";
                }
                if (emocion == "Enojado")
                {
                    ColorEnojado = "#FEE2E2";
                    ColorEnojado_borde = "#FED5D5";
                }
                if (emocion == "Neutro")
                {
                    ColorNeutro = "#F3F4F6";
                    ColorNeutro_borde = "#EBEDF0";
                }
                if (emocion == "Angustiado")
                {
                    ColorAngustiado = "#E0E7FF";
                    ColorAngustiado_borde = "#CCD6FE";
                }
                if (emocion == "Ansioso")
                {
                    ColorAnsioso = "#CCFBF1";
                    ColorAnsioso_borde = "#99F6E4";
                }
            }
            else
            {
                EmocionDiaria.Add(emocion);
                if (emocion == "Feliz")
                {
                    ColorFeliz = "#FF9100";
                }
                if (emocion == "Emocionado")
                {
                    ColorEmocionado = "#FF9100";
                }
                if (emocion == "Cansado")
                {
                    ColorCansado = "#FF9100";
                }
                if (emocion == "Triste")
                {
                    ColorTriste = "#FF9100";
                }
                if (emocion == "Frustrado")
                {
                    ColorFrustrado = "#FF9100";
                }
                if (emocion == "Enojado")
                {
                    ColorEnojado = "#FF9100";
                }
                if (emocion == "Neutro")
                {
                    ColorNeutro = "#FF9100";
                }
                if (emocion == "Angustiado")
                {
                    ColorAngustiado = "#FF9100";
                }
                if (emocion == "Ansioso")
                {
                    ColorAnsioso = "#FF9100";
                }
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
                if (EmocionDiaria.Count == 0 || string.IsNullOrWhiteSpace(DescDia))
                {
                    await Shell.Current.DisplayAlert("Campos en blanco", "No se puede dejar los campos en blanco", "OK");
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
                    CreatedAt = DateTime.Now,

                };

                await App.Database.SaveDiarioAsync(diario);
                await LoadDiariosAsync();

                var payload = new
                {
                    ID_Paciente = Globals.id_usuario, // Usa el id del paciente logueado
                    Emociones = JsonSerializer.Serialize(
                        EmocionDiaria.ToDictionary(e => e, e => 1) // Puedes ajustar el valor según intensidad si lo tienes
                    ),
                    Descripcion = DescDia,
                    Pasos = CantidadPasos,
                    Horas_celular = (int)HorasCelular,
                    Horas_redes = (int)HorasRedes,
                    Horas_Yt = (int)HorasYT,
                    Hora_dormida = HorasSueno,
                    Fecha = DateTime.Now.ToString("yyyy-MM-dd")
                };

                var url = "http://10.0.2.2:5051/api/DiarioEmocional";
                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                using var client = new HttpClient();
                var response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    await Shell.Current.DisplayAlert("¡Listo!", "Diario enviado correctamente.", "OK");
                }
                else
                {
                    var errorMsg = await response.Content.ReadAsStringAsync();
                    await Shell.Current.DisplayAlert("Error", $"No se pudo enviar el diario.\n{errorMsg}", "OK");
                }
            }
            catch (Exception e)
            {
                Error = e.Message;
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
    }
}