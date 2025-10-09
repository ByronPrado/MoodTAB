using CommunityToolkit.Mvvm.ComponentModel; 
using CommunityToolkit.Mvvm.Input;
using MoodTAB.Models;
using MoodTAB.Vistas;
using Plugin.Maui.Calendar.Models;
using System.Globalization;
using MoodTAB.Services;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Drawing;
using Syncfusion.Pdf.Grid;
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
            var diarios = await App.Database.GetDiarioAsync();
            var cultura = new System.Globalization.CultureInfo("es-ES");

            // Agrupar por día
            var grupos = diarios
                .GroupBy(d => DateOnly.FromDateTime(d.CreatedAt))
                .OrderByDescending(g => g.Key);

            using var document = new PdfDocument();
            document.PageSettings.Margins.All = 40;

            // Crear la primera página
            PdfPage page = document.Pages.Add();
            PdfGraphics g = page.Graphics;

            // Intentar cargar el logo
            PdfBitmap? logo = null;
            float logoWidth = 0, logoHeight = 0;
            float logoMargin = 20f;

            try
            {
                using var logoStream = await FileSystem.OpenAppPackageFileAsync("moodtablogo.jpg");

                // Guardar temporalmente en cache
                var tempPath = Path.Combine(FileSystem.CacheDirectory, "temp_logo.jpg");
                using (var fileStream = File.Create(tempPath))
                    await logoStream.CopyToAsync(fileStream);

                // Abrir el logo desde archivo físico como stream
                using var logoFileStream = File.OpenRead(tempPath);
                logo = new PdfBitmap(logoFileStream);

                // Dimensiones deseadas
                logoHeight = 40f;
                logoWidth = logo.Width * (logoHeight / logo.Height);
                            
            }
            catch (Exception ex)
            {
                // Si falla cargar logo, se ignora (solo log)
                System.Diagnostics.Debug.WriteLine($"No se pudo cargar logo: {ex.Message}");
                logo = null;
            }

            // Si se cargó bien  en la página 1 (header)
            if (logo != null)
            {
                float xLogo = page.Graphics.ClientSize.Width - logoMargin - logoWidth;
                float yLogo = logoMargin;
                g.DrawImage(logo, new RectangleF(xLogo, yLogo, logoWidth, logoHeight));
            }

            // Empezar a dibujar el contenido del PDF
            float x = 40, y = 60;
            float cardWidth = g.ClientSize.Width - 80;

            var titleFont = new PdfStandardFont(PdfFontFamily.Helvetica, 16, PdfFontStyle.Bold);
            var dateFont = new PdfStandardFont(PdfFontFamily.Helvetica, 12, PdfFontStyle.Bold);
            var emoFont = new PdfStandardFont(PdfFontFamily.Helvetica, 11, PdfFontStyle.Bold);
            var descFont = new PdfStandardFont(PdfFontFamily.Helvetica, 11);
            var grayBrush = new PdfSolidBrush(new PdfColor(90, 90, 90));

            g.DrawString("Mis Diarios Emocionales", titleFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(x, y));
            y += 30;

            foreach (var grupo in grupos)
            {
                string fechaTexto = grupo.Key.ToString("dddd, dd 'de' MMMM 'de' yyyy", cultura);

                var headerRect = new RectangleF(x - 10, y, cardWidth + 20, 22);
                g.DrawRectangle(new PdfSolidBrush(new PdfColor(240, 240, 240)), headerRect);
                g.DrawString($"📅 {fechaTexto}", dateFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(x, y + 3));
                y += 30;

                foreach (var d in grupo.OrderBy(e => e.CreatedAt))
                {
                    PdfColor emoColor = d.Emocion_Diaria?.ToLower() switch
                    {
                        "feliz" => new PdfColor(46, 204, 113),
                        "triste" => new PdfColor(52, 152, 219),
                        "enojado" => new PdfColor(231, 76, 60),
                        "ansioso" => new PdfColor(241, 196, 15),
                        _ => new PdfColor(149, 165, 166)
                    };

                    // Hora y emoción
                    g.DrawEllipse(new PdfSolidBrush(emoColor), new RectangleF(x, y - 3, 10, 10));
                    g.DrawString(d.CreatedAt.ToString("HH:mm"), emoFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(x + 18, y - 5));
                    g.DrawString(d.Emocion_Diaria, emoFont, new PdfSolidBrush(emoColor), new Syncfusion.Drawing.PointF(x + 70, y - 5));
                    y += 15;

                    // Descripción con wrap automático
                    var descElem = new PdfTextElement(d.Descripcion ?? string.Empty, descFont) { Brush = grayBrush };
                    PdfLayoutResult result = descElem.Draw(page, new RectangleF(x + 18, y, cardWidth - 20, 200));
                    y = result.Bounds.Bottom + 10;

                    // Línea separadora
                    g.DrawLine(new PdfPen(new PdfColor(210, 210, 210), 0.5f), new Syncfusion.Drawing.PointF(x, y), new Syncfusion.Drawing.PointF(x + cardWidth, y));
                    y += 12;

                    // Salto de página si hace falta
                    if (y > page.Graphics.ClientSize.Height - 100)
                    {
                        page = document.Pages.Add();
                        g = page.Graphics;
                        y = 40;

                        // Si el logo existe, dibujarlo también en cada nueva página
                        if (logo != null)
                        {
                            float xLogo = g.ClientSize.Width - logoMargin - logoWidth;
                            float yLogo = logoMargin;
                            g.DrawImage(logo, new RectangleF(xLogo, yLogo, logoWidth, logoHeight));
                            y += logoHeight + 10;
                        }
                    }
                }

                y += 15;
            }

            // Footer: dibujar en cada página
            var footerFont = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
            for (int i = 0; i < document.Pages.Count; i++)
            {
                var p = document.Pages[i];
                p.Graphics.DrawString(
                    $"Exportado: {DateTime.Now.ToString("dd/MM/yyyy HH:mm", cultura)}    Página {i + 1} de {document.Pages.Count}",
                    footerFont,
                    new PdfSolidBrush(new PdfColor(120, 120, 120)),
                    new Syncfusion.Drawing.PointF(40, p.Graphics.ClientSize.Height - 25)
                );
            }

            // Guardar y abrir
            var filePath = Path.Combine(FileSystem.CacheDirectory, "Diarios_Emocionales.pdf");
            using (var stream = File.Create(filePath))
                document.Save(stream);

            document.Close(true);
            await Launcher.OpenAsync(new OpenFileRequest { File = new ReadOnlyFile(filePath) });
        }

    }
}