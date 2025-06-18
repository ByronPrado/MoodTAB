using Microsoft.AspNetCore.Mvc;
using WebConTablas.Models;
using System.Text.Json;

[ApiController]
[Route("api/[controller]")]
public class DiarioEmocionalController : ControllerBase
{
    private readonly AppDbContext _context;

    public DiarioEmocionalController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public IActionResult PostDiario(DiarioEmocional diario)
    {
        int puntosInhibido = 0;
        int puntosExaltado = 0;

        // Analizar emociones
        if (!string.IsNullOrEmpty(diario.Emociones))
        {
            try
            {
                var emociones = JsonSerializer.Deserialize<Dictionary<string, int>>(diario.Emociones);

                foreach (var emocion in emociones)
                {
                    var key = emocion.Key.ToLower();
                    int intensidad = emocion.Value;

                    if ((key.Contains("Triste") || key.Contains("Cansado") || key.Contains("Angustiado")) && intensidad >= 2)
                        puntosInhibido++;

                    if ((key.Contains("Emocionado") || key.Contains("Enojado") || key.Contains("Frustrado") || key.Contains("Ansioso") )  && intensidad >= 2)
                        puntosExaltado++;
                }
            }
            catch
            {
                return BadRequest("Formato de emociones inválido");
            }
        }

        // Evaluar pasos
        if (diario.Pasos.HasValue)
        {
            if (diario.Pasos < 2000)
                puntosInhibido++;
            else if (diario.Pasos > 10000)
                puntosExaltado++;
        }

        // Evaluar uso de celular
        if (diario.Horas_celular.HasValue && diario.Horas_celular > 5)
        {
            puntosInhibido++;
            puntosExaltado++;
        }

        // Evaluar uso de redes sociales
        if (diario.Horas_redes.HasValue && diario.Horas_redes > 3)
        {
            puntosInhibido++;
            puntosExaltado++;
        }

        // Evaluar hora dormida (como duración de sueño)
        // Necesitamos saber cuánto durmió. Asumimos que 'Hora_dormida' es string "HH:mm", hora en que se durmió
        // y se despertó a las 08:00 am, entonces:
        if (TimeSpan.TryParse(diario.Hora_dormida, out TimeSpan horaDormida))
        {
            TimeSpan horaDespertar = TimeSpan.FromHours(8); // 8:00 AM
            TimeSpan duracionSueño = horaDespertar > horaDormida
                ? horaDespertar - horaDormida
                : (TimeSpan.FromHours(24) - horaDormida + horaDespertar);

            if (duracionSueño.TotalHours < 3)
                puntosExaltado++;
            else if (duracionSueño.TotalHours > 10)
                puntosInhibido++;
        }

        // Determinar estado final
        if (puntosExaltado >= 3 && puntosExaltado >= puntosInhibido)
            diario.Estado = "exaltado";
        else if (puntosInhibido >= 3)
            diario.Estado = "inhibido";
        else
            diario.Estado = "normal";

        _context.DiariosEmocionales.Add(diario);
        _context.SaveChanges();

        return Ok(new { diario.ID_Diario, diario.Estado });
    }
}