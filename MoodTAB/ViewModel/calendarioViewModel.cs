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
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.Maui.Graphics.Text;




namespace MoodTAB.ViewModel
{
    public class Model
    {
        public string Month { get; set; }

        public double Target { get; set; }

        public Model(string xValue, double yValue)
        {
            Month = xValue;
            Target = yValue;
        }
        
    }
    
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


        private ObservableCollection<Model> _data;
        public ObservableCollection<Model> Data
        {
            get => _data;
            set
            {
                _data = value;
                OnPropertyChanged(nameof(Data)); // o RaisePropertyChanged("Data")
            }
        }
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

            Task.Run(async () => await InicializarDatosGraficoAsync());

        }

        private async Task InicializarDatosGraficoAsync()
        {
            var data = await CrearDatosGraficoSemanaAsync();
            MainThread.BeginInvokeOnMainThread(() => Data = data);
        }
        public async Task<ObservableCollection<Model>> CrearDatosGraficoSemanaAsync()
        {
            try
            {
                DateTime hoy = DateTime.Today;

                // Calcular lunes de la semana actual
                int diff = hoy.DayOfWeek - DayOfWeek.Monday;
                if (diff < 0) diff += 7;
                DateTime lunes = hoy.AddDays(-diff);

                // Generar los 7 días (lunes a domingo)
                var semana = Enumerable.Range(0, 7)
                                       .Select(i => lunes.AddDays(i))
                                       .ToList();

                if (App.Database == null)
                {
                    System.Diagnostics.Debug.WriteLine(" App.Database no inicializado");
                    return new ObservableCollection<Model>();
                }

                // Obtener registros
                var diarios = await App.Database.GetDiariosDiasAnterioresAsync(7) ?? new List<Diario>();
                System.Diagnostics.Debug.WriteLine($"Se obtuvieron {diarios.Count} registros");

                foreach (var d in diarios)
                    System.Diagnostics.Debug.WriteLine($"→ {d.CreatedAt} | {d.Horas_Sueno}");

                // Agrupar registros por día
                var agrupado = diarios
                    .Where(d => d.CreatedAt != default)
                    .GroupBy(d => d.CreatedAt.Date)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Average(d =>
                        {
                            if (double.TryParse(d.Horas_Sueno, out double valor))
                                return valor;
                            return 0;
                        })
                    );

                System.Diagnostics.Debug.WriteLine(" Agrupado por día:");
                foreach (var kv in agrupado)
                    System.Diagnostics.Debug.WriteLine($"{kv.Key:dd/MM/yyyy} → {kv.Value}");

                // Crear datos para gráfico
                var data = new ObservableCollection<Model>();

                foreach (var dia in semana)
                {
                    double valor = agrupado.ContainsKey(dia) ? agrupado[dia] : 0;
                    string etiqueta = dia.ToString("dd/MM");
                    data.Add(new Model(etiqueta, valor));

                    System.Diagnostics.Debug.WriteLine($"📊 {etiqueta} = {valor}\n {data.Last().Month},{data.Last().Target}");
                }

                return data;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creando datos: {ex}");
                return new ObservableCollection<Model>();
            }
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
            //var diarios = await App.Database.GetDiarioAsync();
            var diarios = await App.Database.GetDiariosMesActualAsync();

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

            g.DrawString("Horas de Sueño semana", titleFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(x, y));
            y += 30;

// Insertar el gráfico de barras generado a partir de los datos
// -------------------------------------------------------------
try
{
    if (Data != null && Data.Any())
    {
        float chartHeight = 200f; // altura deseada del gráfico
        float chartWidth = cardWidth;
        float barSpacing = 10f;
        float barWidth = (chartWidth - (Data.Count - 1) * barSpacing) / Data.Count;
        float maxY = (float)Data.Max(d => d.Target); // límite Y basado en el valor máximo
        float scaleFactor = chartHeight / (maxY > 0 ? maxY : 1); // escalar alturas

        float margin = 10f;
        float chartX = x + margin;
        float chartY = y + margin;

        // Dibujar marco
        g.DrawRectangle(new PdfPen(PdfBrushes.Gray, 1f), new RectangleF(x, y, chartWidth, chartHeight + 2 * margin + 40));

        // Líneas de grilla horizontal y valores Y
        int gridLines = 5;
        for (int i = 0; i <= gridLines; i++)
        {
            float yPos = chartY + chartHeight - (i * chartHeight / gridLines);
            g.DrawLine(new PdfPen(new PdfColor(200, 200, 200), 0.5f),
                new Syncfusion.Drawing.PointF(chartX, yPos),
                new Syncfusion.Drawing.PointF(chartX + chartWidth - 2 * margin, yPos));

            float yValue = i * maxY / gridLines;
            g.DrawString(yValue.ToString("0.##"), descFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(x, yPos - 7));
        }

        // Dibujar cada barra y etiquetas rotadas
        float currentX = chartX;
        float labelAngle = -45f; // rotar etiquetas 45° hacia la izquierda
        foreach (var punto in Data)
        {
            float barHeight = (float)punto.Target * scaleFactor;
            g.DrawRectangle(new PdfSolidBrush(new PdfColor(59, 130, 246)),
                new RectangleF(currentX, chartY + chartHeight - barHeight, barWidth, barHeight));

            // Etiqueta de eje X con rotación
            g.Save();
            g.TranslateTransform(currentX + barWidth / 2, chartY + chartHeight + 25); // punto de rotación
            g.RotateTransform(labelAngle);
            g.DrawString(punto.Month, descFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(0, 0));
            g.Restore();

            currentX += barWidth + barSpacing;
        }

        // Leyenda
        float legendX = chartX + chartWidth - 100;
        float legendY = chartY + chartHeight + 25;
        g.DrawRectangle(new PdfSolidBrush(new PdfColor(59, 130, 246)), new RectangleF(legendX, legendY, 12, 12));
        g.DrawString("Horas de sueño", descFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(legendX + 18, legendY - 2));

        y += chartHeight + 70; // actualizar y después del gráfico
    }
}
catch (Exception ex)
{
    System.Diagnostics.Debug.WriteLine($"Error generando gráfico en PDF: {ex}");
}


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