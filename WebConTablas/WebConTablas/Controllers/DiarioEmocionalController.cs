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
        List<string> emocionesList = new();
    if (!string.IsNullOrEmpty(diario.Emociones))
    {
        try
        {
            // Si es un string separado por comas, lo convertimos a lista
            emocionesList = diario.Emociones
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(e => e.Trim())
                .ToList();
        }
        catch
        {
            return BadRequest("Formato de emociones inválido");
        }
    }

    foreach (var emocion in emocionesList)
    {
        var key = emocion.ToLower();

        // Asumimos intensidad 1
        int intensidad = 1;

        if ((key.Contains("triste") || key.Contains("cansado") || key.Contains("angustiado")) && intensidad >= 1)
            puntosInhibido++;

        if ((key.Contains("emocionado") || key.Contains("enojado") || key.Contains("frustrado") || key.Contains("ansioso")) && intensidad >= 1)
            puntosExaltado++;
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
        if (int.TryParse(diario.Hora_dormida, out int horaDormida))
        {
            if (horaDormida < 3)
                puntosExaltado++;
            else if (horaDormida > 10)
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