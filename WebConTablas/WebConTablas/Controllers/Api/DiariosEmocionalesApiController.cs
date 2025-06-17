using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebConTablas.Models;

[ApiController]
[Route("api/[controller]")]
public class DiariosEmocionalesApiController : ControllerBase
{
    private readonly AppDbContext _context;
    public DiariosEmocionalesApiController(AppDbContext context) => _context = context;

    // GET: api/DiariosEmocionales/ByPaciente/1
    [HttpGet("ByPaciente/{idPaciente}")]
    public async Task<IActionResult> GetByPaciente(int idPaciente)
    {
        var diarios = await _context.DiariosEmocionales
            .Where(d => d.ID_Paciente == idPaciente)
            .ToListAsync();
        return Ok(diarios);
    }

    // POST: api/DiariosEmocionales
    [HttpPost]
    public async Task<IActionResult> Post(DiarioEmocional diario)
    {
        diario.Estado = CalcularEstado(diario);
        _context.DiariosEmocionales.Add(diario);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetByPaciente), new { idPaciente = diario.ID_Paciente }, diario);
    }

    private string CalcularEstado(DiarioEmocional diario)
    {
        // Algoritmo ejemplo, personaliza según tus reglas:
        if ((diario.Pasos ?? 0) > 10000 || (diario.Emociones?.Contains("feliz") ?? false))
            return "exitado";
        if ((diario.Pasos ?? 0) < 2000 || (diario.Emociones?.Contains("triste") ?? false))
            return "inhibido";
        return "normal";
    }
}