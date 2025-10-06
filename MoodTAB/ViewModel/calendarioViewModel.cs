using CommunityToolkit.Mvvm.ComponentModel; 
using CommunityToolkit.Mvvm.Input;
using MoodTAB.Models;
using MoodTAB.Vistas;
using Plugin.Maui.Calendar.Models;
using System.Globalization;
using MoodTAB.Services;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using System.IO;

namespace MoodTAB.ViewModel
{
    public partial class CalendarioViewModel : ObservableObject
    {
        [ObservableProperty]
        public EventCollection events = new EventCollection();
        [ObservableProperty]
        private CultureInfo cultura = new("es-ES");
        public string logtext = "";
        //"Week"
        [ObservableProperty]
        private DateTime shownDate = DateTime.Today;

        public IRelayCommand<DateTime> MesCambiadoCommand { get; }
        public IRelayCommand<DateTime> DiaTocadoCommand { get; }
        public IRelayCommand<string> CambiarMesCommand { get; }
        private int setCalendarioLayout = 0;

        public CalendarioViewModel()
        {
            CargarEventos();
            // inicializar comandos
            MesCambiadoCommand = new RelayCommand<DateTime>(fecha => ShownDate = fecha);
            DiaTocadoCommand = new RelayCommand<DateTime>(async fecha => OnDiaTocado(fecha));
            CambiarMesCommand = new RelayCommand<string>(delta =>
            {
                if (int.TryParse(delta, out var d)) ShownDate = ShownDate.AddMonths(d);
            });
        }

        private async void OnDiaTocado(DateTime fecha)
        {
            // Primero intentar leer desde Events (si existe)
            if (Events != null && Events.ContainsKey(fecha.Date) && Events[fecha.Date] is System.Collections.IEnumerable col)
            {
                var diarios = col.Cast<Diario>().ToList();
                //aqui debo hace rla logica de cambiar a lista diario o detalle
                if (diarios.Count <= 1)
                {
                    await App.Current.MainPage.Navigation.PushAsync(new DetalleDiarioPage(diarios.First()));
                    return;
                }
                else
                {
                    var stepService = IPlatformApplication.Current?.Services?.GetService<IStepCounterService>();
                    await Application.Current.MainPage.Navigation.PushAsync(new ListaDiarioPage(stepService, fecha));
                    return;
                }

            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("No hay Registro", $"No hay diario registrado este dia\n", "No");
                return;
            }
        }

        private async void CargarEventos()
        {
            var diarios = await App.Database.GetDiarioAsync();

            // Limpio primero
            Events.Clear();

            foreach (var diario in diarios)
            {
                var fecha = diario.CreatedAt.Date;

                if (!Events.ContainsKey(fecha))
                    Events[fecha] = new List<Diario>();

                (Events[fecha] as List<Diario>)!.Add(diario);
            }
        }
        public async Task ExportarPDF()
        {
            using var document = new PdfDocument();
            var page = document.Pages.Add();

            var font = new PdfStandardFont(PdfFontFamily.Helvetica, 12);
            page.Graphics.DrawString("Mis diarios emocionales", font, PdfBrushes.Black, 0, 0);

            var diarios = await App.Database.GetDiarioAsync();
            float y = 20;
            foreach (var diario in diarios)
            {
                page.Graphics.DrawString(
                    $"{diario.CreatedAt:dd/MM/yyyy} - {diario.Descripcion}: {diario.Emocion_Diaria}",
                    font,
                    PdfBrushes.Black,
                    0,
                    y
                );
                y += 20;
            }

            var filePath = Path.Combine(FileSystem.CacheDirectory, "Diarios.pdf");
            using var stream = File.Create(filePath);
            document.Save(stream);
            document.Close(true);

            await Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(filePath)
            });
        }
   
    }
}