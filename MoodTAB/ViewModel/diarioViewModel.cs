using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoodTAB.Models;
using MoodTAB.Vistas;
using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MoodTAB.Services;

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
        bool anotado;

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
            anotado = false;

            ColorFeliz ="#512BD4";
            ColorEmocionado ="#512BD4";
            ColorCansado ="#512BD4";
            ColorTriste ="#512BD4";
            ColorFrustrado ="#512BD4";
            ColorEnojado ="#512BD4";
            ColorNeutro ="#512BD4";
            ColorAngustiado ="#512BD4";
            ColorAnsioso ="#512BD4";


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
                    ColorFeliz = "#512BD4";
                }
                if (emocion == "Emocionado")
                {
                    ColorEmocionado = "#512BD4";
                }
                if (emocion == "Cansado")
                {
                    ColorCansado = "#512BD4";
                }
                if (emocion == "Triste")
                {
                    ColorTriste = "#512BD4";
                }
                if (emocion == "Frustrado")
                {
                    ColorFrustrado = "#512BD4";
                }
                if (emocion == "Enojado")
                {
                    ColorEnojado = "#512BD4";
                }
                if (emocion == "Neutro")
                {
                    ColorNeutro = "#512BD4";
                }
                if (emocion == "Angustiado")
                {
                    ColorAngustiado = "#512BD4";
                }
                if (emocion == "Ansioso")
                {
                    ColorAnsioso = "#512BD4";
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
            var stats = UsageStatsHelper.GetAppUsageStats();
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
                if (Anotado)
                {
                    await Shell.Current.DisplayAlert("Diaro ya subido", "Usted ya subió su diario emocional hoy", "OK");
                    return;
                }
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
                Anotado = true;
                await LoadDiariosAsync();
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

        [RelayCommand]
        private void CambiarAnotado()
        {
            if (Anotado)
            {
                Anotado = false;
            }
            else
            {
                Anotado = true;
            }
        }
    }
}