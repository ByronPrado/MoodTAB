using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using MoodTAB.Models;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Drawing;
namespace MoodTAB.ViewModel
{
    public partial class CuestionarioRespondidoViewModel : ObservableObject
    {
        [ObservableProperty]
        string titulo;

        [ObservableProperty]
        string descripcion;

        [ObservableProperty]
        ObservableCollection<PreguntaCompletada> preguntas = new();
        [ObservableProperty]
        DateTime fechaRespondido;
        public CuestionarioCompletado Cuestionario { get; }
        public CuestionarioRespondidoViewModel(CuestionarioCompletado cuestionario)
        {
            Cuestionario = cuestionario;
            Titulo = cuestionario.Formulario.Titulo;
            Descripcion = cuestionario.Formulario.Descripcion;
            FechaRespondido = cuestionario.Fecha_Asignacion;
            if (cuestionario.Formulario.Preguntas != null)
            {
                foreach (var p in cuestionario.Formulario.Preguntas)
                {
                    Preguntas.Add(p);
                }
            }
        }

        [RelayCommand]
        public async Task ExportarPdf()
        {
            var cultura = new System.Globalization.CultureInfo("es-Es");

            using var document = new PdfDocument();
            document.PageSettings.Margins.All = 40;

            PdfPage page = document.Pages.Add();
            PdfGraphics g = page.Graphics;

            // Header con logo
            PdfBitmap? logo = null;
            float logoWidth = 0, logoHeight = 0;
            float logoMargin = 20f;

            try
            {
                using var logoStream = await FileSystem.OpenAppPackageFileAsync("moodtablogo.jpg");
                var tempPath = Path.Combine(FileSystem.CacheDirectory, "temp_logo.jpg");
                using (var fileStream = File.Create(tempPath))
                    await logoStream.CopyToAsync(fileStream);

                using var logoFileStream = File.OpenRead(tempPath);
                logo = new PdfBitmap(logoFileStream);

                logoHeight = 40f;
                logoWidth = logo.Width * (logoHeight / logo.Height);
            }
            catch { logo = null; }

            if (logo != null)
            {
                float xLogo = page.Graphics.ClientSize.Width - logoMargin - logoWidth;
                float yLogo = logoMargin;
                g.DrawImage(logo, new RectangleF(xLogo, yLogo, logoWidth, logoHeight));
            }

            // Estilos
            float x = 20, y = 60;
            float cardWidth = g.ClientSize.Width - 60;

            var titleFont = new PdfStandardFont(PdfFontFamily.Helvetica, 16, PdfFontStyle.Bold);
            var dateFont = new PdfStandardFont(PdfFontFamily.Helvetica, 12, PdfFontStyle.Bold);
            var questionFont = new PdfStandardFont(PdfFontFamily.Helvetica, 11, PdfFontStyle.Bold);
            var answerFont = new PdfStandardFont(PdfFontFamily.Helvetica, 11);
            var grayBrush = new PdfSolidBrush(new PdfColor(90, 90, 90));

            // Título
            g.DrawString(Titulo, titleFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(x, y));
            y += 28;

            // Fecha
            string fechaTexto = FechaRespondido.ToString("dddd, dd 'de' MMMM 'de' yyyy", cultura);
            g.DrawString($"Asignado el: {fechaTexto}", dateFont, PdfBrushes.Gray, new Syncfusion.Drawing.PointF(x, y));
            y += 30;

            // Descripción
            if (!string.IsNullOrWhiteSpace(Descripcion))
            {
                var descElem = new PdfTextElement(Descripcion, answerFont) { Brush = grayBrush };
                var descResult = descElem.Draw(page, new RectangleF(x, y, cardWidth, 200));
                y = descResult.Bounds.Bottom + 20;
            }

            // Preguntas y respuestas
            foreach (var p in Preguntas)
            {
                // Pregunta
                var preguntaElem = new PdfTextElement($"• {p.Contenido}", questionFont) { Brush = PdfBrushes.Black };
                var preguntaRes = preguntaElem.Draw(page, new RectangleF(x, y, cardWidth, 100));
                y = preguntaRes.Bounds.Bottom + 6;

                // Respuesta
                var respuestaTexto = string.IsNullOrWhiteSpace(p.Respuesta) ? "(sin respuesta)" : p.Respuesta;
                var respElem = new PdfTextElement(respuestaTexto, answerFont) { Brush = grayBrush };
                var respRes = respElem.Draw(page, new RectangleF(x + 20, y, cardWidth - 20, 200));
                y = respRes.Bounds.Bottom + 12;

                // Línea separadora
                g.DrawLine(new PdfPen(new PdfColor(210, 210, 210), 0.5f),
                    new Syncfusion.Drawing.PointF(x, y), new Syncfusion.Drawing.PointF(x + cardWidth, y));
                y += 10;

                // Salto de página
                if (y > page.Graphics.ClientSize.Height - 100)
                {
                    page = document.Pages.Add();
                    g = page.Graphics;
                    y = 60;

                    if (logo != null)
                    {
                        float xLogo = g.ClientSize.Width - logoMargin - logoWidth;
                        float yLogo = logoMargin;
                        g.DrawImage(logo, new RectangleF(xLogo, yLogo, logoWidth, logoHeight));
                        y += logoHeight + 20;
                    }
                }
            }

            // Footer
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
            var filePath = Path.Combine(FileSystem.CacheDirectory, "Cuestionario_Respondido.pdf");
            using (var stream = File.Create(filePath))
                document.Save(stream);

            document.Close(true);
            await Launcher.OpenAsync(new OpenFileRequest { File = new ReadOnlyFile(filePath) });
        }
    }
}
