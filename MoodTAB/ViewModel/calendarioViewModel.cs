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
        //Calendario
        [ObservableProperty]
        public EventCollection events = new EventCollection();
        [ObservableProperty]
        private CultureInfo cultura = new("es-ES");
        //"Week"
        [ObservableProperty]
        private DateTime shownDate = DateTime.Today;

        public IRelayCommand<DateTime> MesCambiadoCommand { get; }
        public IRelayCommand<DateTime> DiaTocadoCommand { get; }
        public IRelayCommand<string> CambiarMesCommand { get; }
        private int setCalendarioLayout = 0;

        //Horas Sueño
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
        //calidad Sueño
        private ObservableCollection<Model> _calidadSueno;
        public ObservableCollection<Model> CalidadSueno
        {
            get => _calidadSueno;
            set
            {
                _calidadSueno = value;
                OnPropertyChanged(nameof(CalidadSueno)); // o RaisePropertyChanged("Data")
            }
        }
        
        public string logtext = "";

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
            var calidadSleep = await CrearDatosGraficoSuenoAsync();
            MainThread.BeginInvokeOnMainThread(() => {
                Data = data;
                CalidadSueno = calidadSleep;});
        }


        public async Task<ObservableCollection<Model>> CrearDatosGraficoSemanaAsync()
        {
            try
            {
                DateTime hoy = DateTime.Today;

                // Generar los 7 días (lunes a domingo)
                var ultimos7Dias = Enumerable.Range(0, 7)
                                       .Select(i => hoy.AddDays(-6+i))
                                       .ToList();

                if (App.Database == null)
                {
                    System.Diagnostics.Debug.WriteLine(" App.Database no inicializado");
                    return new ObservableCollection<Model>();
                }

                // Obtener registros
                var diarios = await App.Database.GetDiariosDiasAnterioresAsync(7) ?? new List<Diario>();
                System.Diagnostics.Debug.WriteLine($"Se obtuvieron {diarios.Count} registros");

                foreach (var d in diarios) System.Diagnostics.Debug.WriteLine($"→ {d.CreatedAt} | {d.Horas_Sueno}");

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

                foreach (var dia in ultimos7Dias)
                {
                    double valor = agrupado.ContainsKey(dia) ? agrupado[dia] : 0;
                    //string etiqueta = dia.ToString("dd/MM");
                    string etiqueta = dia.ToString("ddd",cultura);

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
        
        public async Task<ObservableCollection<Model>>CrearDatosGraficoSuenoAsync()
        {
            try
            {
                DateTime hoy = DateTime.Today;

                var ultimos7Dias = Enumerable.Range(0, 7)
                                    .Select(i => hoy.AddDays(-6+i))
                                    .ToList();

                if (App.Database == null)
                    return new ObservableCollection<Model>();

                var diarios = await App.Database.GetDiariosDiasAnterioresAsync(7) ?? new List<Diario>();

                // Agrupar emociones por día → promedio de todos los valores del string
                var agrupado = diarios
                    .Where(d => d.CreatedAt != default && !string.IsNullOrWhiteSpace(d.Emocion_Diaria))
                    .GroupBy(d => d.CreatedAt.Date)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Average(d =>
                        {
                            try
                            {
                                return ObtenerValorSlider(d.Emocion_Diaria, 4);
                            }
                            catch { return 0; }
                        })
                    );
                System.Diagnostics.Debug.WriteLine($"Agrupado:{agrupado}");
                
                var data = new ObservableCollection<Model>();

                foreach (var dia in ultimos7Dias)
                {
                    double valor = agrupado.ContainsKey(dia) ? agrupado[dia] : 0;
                    string etiqueta = dia.ToString("ddd", cultura);

                    data.Add(new Model(etiqueta, valor));
                    System.Diagnostics.Debug.WriteLine($"📊 calidadsueño {etiqueta} = {valor}");
                }

                return data;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creando datos sueño: {ex}");
                return new ObservableCollection<Model>();
            }
        }


        //con esto sacamos los valores de los sliders pa hacer mas graficos a futuro.
        public static int ObtenerValorSlider(string emociones, int posicion)
        {
            if (string.IsNullOrWhiteSpace(emociones))
                return 0;

            var partes = emociones.Split(',', StringSplitOptions.RemoveEmptyEntries);

            // Validar que la posición exista (1‑based)
            if (posicion <= 0 || posicion > partes.Length)
                return 0;

            return int.TryParse(partes[posicion - 1], out int valor) ? valor : 0;
        }

        //funcion para calendario, logica de tocar un dia y cambiar vista en funcion de # diarios
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
                //await Application.Current.MainPage.DisplayAlert("No hay Registro", $"No hay diario registrado este dia\n", "No");
                return;
            }
        }

        //funcion para mostrar los diarios en el calendario
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

        //funcion Para crear el pdf en funcion de los datos.
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
            float x = 20, y = 60;
            float cardWidth = g.ClientSize.Width - 60;

            var titleFont = new PdfStandardFont(PdfFontFamily.Helvetica, 16, PdfFontStyle.Bold);
            var dateFont = new PdfStandardFont(PdfFontFamily.Helvetica, 12, PdfFontStyle.Bold);
            var emoFont = new PdfStandardFont(PdfFontFamily.Helvetica, 11, PdfFontStyle.Bold);
            var descFont = new PdfStandardFont(PdfFontFamily.Helvetica, 11);
            var grayBrush = new PdfSolidBrush(new PdfColor(90, 90, 90));

            g.DrawString("Monitoreo de Sueño Ultimos 7 días", titleFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(x, y));
            y += 30;

            // Insertar el gráfico de barras generado a partir de los datos
            try
            {
                if (Data != null && Data.Any())
                {
                    // Tamaños base
                    float chartWidth = cardWidth*0.9f;
                    float chartHeight = 180f;

                    // Márgenes externos (marco)
                    float outerMargin = 10f;

                    // Márgenes internos (espacio entre borde del marco y el gráfico real)
                    float innerMargin = 20f;

                    float barSpacing = 6f;
                    float barWidth = (chartWidth - (Data.Count - 1) * barSpacing - 2 * innerMargin) / Data.Count;

                    float maxY = (float)Data.Max(d => d.Target);
                    float scaleFactor = chartHeight / (maxY > 0 ? maxY : 1);

                    // Coordenadas
                    float frameX = x + outerMargin +20f;
                    float frameY = y + outerMargin;
                    float chartX = frameX + innerMargin+10f;
                    float chartY = frameY + innerMargin;

                    // Marco general
                    float frameHeight = chartHeight + innerMargin * 2 + 60; // espacio extra abajo para etiquetas
                    g.DrawRectangle(new PdfPen(PdfBrushes.Gray, 1f), new RectangleF(frameX, frameY, chartWidth, frameHeight));

                    // Líneas de grilla horizontal y valores del eje Y
                    int gridLines = 5;
                    for (int i = 0; i <= gridLines; i++)
                    {
                        float yPos = chartY + chartHeight - (i * chartHeight / gridLines);
                        g.DrawLine(new PdfPen(new PdfColor(220, 220, 220), 0.5f),
                            new Syncfusion.Drawing.PointF(chartX, yPos),
                            new Syncfusion.Drawing.PointF(chartX + chartWidth - 2 * innerMargin, yPos));

                        float yValue = i * maxY / gridLines;
                        g.DrawString(yValue.ToString("0.##"), emoFont, PdfBrushes.Black,
                            new Syncfusion.Drawing.PointF(frameX +15f, yPos));
                    }

                    // Barras y etiquetas del eje X
                    float currentX = chartX;
                    float labelAngle = -45f;
                    foreach (var punto in Data)
                    {
                        float barHeight = (float)punto.Target * scaleFactor;
                        g.DrawRectangle(new PdfSolidBrush(new PdfColor(59, 130, 246)),
                            new RectangleF(currentX, chartY + chartHeight - barHeight, barWidth, barHeight));

                        // Etiqueta X rotada
                        float labelOffset = 18 + emoFont.Size;
                        g.Save();
                        g.TranslateTransform(currentX + barWidth / 2, chartY + chartHeight + labelOffset);
                        g.RotateTransform(labelAngle);
                        g.DrawString(punto.Month, emoFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(0, 0));
                        g.Restore();

                        currentX += barWidth + barSpacing;
                    }


                    // Eje Y: “Horas de sueño”
                    g.Save();
                    g.TranslateTransform(frameX+5, chartY + chartHeight / 2);
                    g.RotateTransform(-90);
                    g.DrawString("Horas de sueño", emoFont, PdfBrushes.Black,
                        new Syncfusion.Drawing.PointF(-50,-emoFont.Size / 2));
                    g.Restore();

                    // Eje X: “Días de la semana”
                    float ejeXTextY = chartY + chartHeight + 60;
                    g.DrawString("Días de la semana", emoFont, PdfBrushes.Black,
                        new Syncfusion.Drawing.PointF(chartX + (chartWidth / 2) - 60, ejeXTextY));


                    // Leyenda
                    float legendBoxSize = 12f;
                    float legendSpacing = 5f;
                    float legendMarginTop = 45f;

                    float legendX = chartX + chartWidth - 140;
                    float legendY = chartY + chartHeight + legendMarginTop;

                    g.DrawRectangle(new PdfSolidBrush(new PdfColor(59, 130, 246)),
                        new RectangleF(legendX, legendY, legendBoxSize, legendBoxSize));

                    g.DrawString("Horas de sueño", emoFont, PdfBrushes.Black,
                        new Syncfusion.Drawing.PointF(legendX + legendBoxSize + legendSpacing, legendY - 1));

                    // Actualiza posición vertical para lo siguiente
                    y += frameHeight + 40;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error generando gráfico en PDF: {ex}");
            }

            //grafico Calidad de sueño
            try
            {
                if (CalidadSueno != null && CalidadSueno.Any())
                {
                    //base
                    float chartWidth = cardWidth*0.9f;
                    float chartHeight = 180f;
                    //marco
                    float outerMargin = 10f;
                    //margenes internos
                    float innerMargin = 20f;
                    float barSpacing = 6f;
                    float barWidth = (chartWidth - (CalidadSueno.Count - 1) * barSpacing - 2 * innerMargin) / CalidadSueno.Count;

                    float maxY = (float)CalidadSueno.Max(d => d.Target);
                    float scaleFactor = chartHeight / (maxY > 0 ? maxY : 1);
                    //coordenadas
                    float frameX = x + outerMargin + 20f;
                    float frameY = y + outerMargin;
                    float chartX = frameX + innerMargin+10f;
                    float chartY = frameY + innerMargin;
                    // marco general
                    float frameHeight = chartHeight + innerMargin * 2 + 60;
                    g.DrawRectangle(new PdfPen(PdfBrushes.Gray, 1f), new RectangleF(frameX, frameY, chartWidth, frameHeight));

                    int gridLines = 5;
                    for (int i = 0; i <= gridLines; i++)
                    {
                        float yPos = chartY + chartHeight - (i * chartHeight / gridLines);
                        g.DrawLine(new PdfPen(new PdfColor(220, 220, 220), 0.5f),
                            new Syncfusion.Drawing.PointF(chartX, yPos),
                            new Syncfusion.Drawing.PointF(chartX + chartWidth - 2 * innerMargin, yPos));

                        float yValue = i * maxY / gridLines;
                        g.DrawString(yValue.ToString("0.##"), emoFont, PdfBrushes.Black,
                            new Syncfusion.Drawing.PointF(frameX + 15f, yPos));
                    }

                    float currentX = chartX;
                    float labelAngle = -45f;
                    foreach (var punto in CalidadSueno)
                    {
                        float barHeight = (float)punto.Target * scaleFactor;
                        g.DrawRectangle(new PdfSolidBrush(new PdfColor(34, 197, 94)), // verde para diferenciar
                            new RectangleF(currentX, chartY + chartHeight - barHeight, barWidth, barHeight));

                        float labelOffset = 18 + emoFont.Size;
                        g.Save();
                        g.TranslateTransform(currentX + barWidth / 2, chartY + chartHeight + labelOffset);
                        g.RotateTransform(labelAngle);
                        g.DrawString(punto.Month, emoFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(0, 0));
                        g.Restore();

                        currentX += barWidth + barSpacing;
                    }

                    // Eje Y: “Calidad de sueño”
                    g.Save();
                    g.TranslateTransform(frameX + 5, chartY + chartHeight / 2);
                    g.RotateTransform(-90);
                    g.DrawString("Calidad de sueño", emoFont, PdfBrushes.Black,
                        new Syncfusion.Drawing.PointF(-50, -emoFont.Size / 2));
                    g.Restore();

                    // Eje X: “Días de la semana”
                    float ejeXTextY = chartY + chartHeight + 60;
                    g.DrawString("Días de la semana", emoFont, PdfBrushes.Black,
                        new Syncfusion.Drawing.PointF(chartX + (chartWidth / 2) - 60, ejeXTextY));

                    // Leyenda
                    float legendBoxSize = 12f;
                    float legendSpacing = 5f;
                    float legendMarginTop = 45f;

                    float legendX = chartX + chartWidth - 140;
                    float legendY = chartY + chartHeight + legendMarginTop;

                    g.DrawRectangle(new PdfSolidBrush(new PdfColor(34, 197, 94)),
                        new RectangleF(legendX, legendY, legendBoxSize, legendBoxSize));

                    g.DrawString("Calidad de sueño", emoFont, PdfBrushes.Black,
                        new Syncfusion.Drawing.PointF(legendX + legendBoxSize + legendSpacing, legendY - 1));

                    y += frameHeight + 40;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error generando gráfico de calidad de sueño en PDF: {ex}");
            }
            
            //
            if (y > page.Graphics.ClientSize.Height - 200)
            {
                page = document.Pages.Add();
                g = page.Graphics;
                y = 60;

                // Redibujar logo en el encabezado si existe
                if (logo != null)
                {
                    float xLogo = g.ClientSize.Width - logoMargin - logoWidth;
                    float yLogo = logoMargin;
                    g.DrawImage(logo, new RectangleF(xLogo, yLogo, logoWidth, logoHeight));
                    y += logoHeight + 20;
                }
            }

            //Lista de los diarios emocionales
             
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
                    var sliderCalidad = ObtenerValorSlider(d.Emocion_Diaria, 4);
                    PdfColor emoColor = sliderCalidad switch
                    {
                        < 3 => new PdfColor(231, 76, 60),   // rojo calidad baja
                        < 5 => new PdfColor(241, 196, 15),  // amarillo 
                        < 7 => new PdfColor(52, 152, 219),  // azul 
                        _   => new PdfColor(46, 204, 113)   // verde para valores altos
                    };

                    // Hora y emoción
                    g.DrawEllipse(new PdfSolidBrush(emoColor), new RectangleF(x, y - 3, 10, 10));
                    g.DrawString(d.CreatedAt.ToString("HH:mm"), emoFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(x + 18, y - 5));
                    g.DrawString(d.Emocion_Diaria, emoFont, new PdfSolidBrush(emoColor), new Syncfusion.Drawing.PointF(x + 70, y - 5));
                    y += 15;

                    // Descripción con wrap automático
                    var descElem = new PdfTextElement(d.Descripcion ?? string.Empty, emoFont) { Brush = grayBrush };
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