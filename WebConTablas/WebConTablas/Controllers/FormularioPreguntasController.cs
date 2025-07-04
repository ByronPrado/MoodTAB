using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebConTablas.Models;

public class FormularioPreguntasController : Controller
{
    private readonly AppDbContext _context;
    public FormularioPreguntasController(AppDbContext context) => _context = context;

    // GET: FormularioPreguntas/Asignar/5
    public IActionResult Asignar(int id)
    {
        var formulario = _context.Formularios
            .Include(f => f.Preguntas)
            .FirstOrDefault(f => f.ID_Formulario == id);

        // Obtén los IDs de las preguntas ya asignadas
        var preguntasAsignadasIds = formulario.Preguntas.Select(fp => fp.ID_Pregunta).ToList();

        // Filtra las preguntas para mostrar solo las NO asignadas
        var preguntasNoAsignadas = _context.Preguntas
            .Where(p => !preguntasAsignadasIds.Contains(p.ID_Pregunta))
            .ToList();

        ViewBag.Formulario = formulario;
        ViewBag.Preguntas = preguntasNoAsignadas;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Asignar(int id, int[] preguntasSeleccionadas)
    {
        foreach (var idPregunta in preguntasSeleccionadas)
        {
            if (!_context.FormularioPreguntas.Any(fp => fp.ID_Formulario == id && fp.ID_Pregunta == idPregunta))
            {
                _context.FormularioPreguntas.Add(new FormularioPregunta
                {
                    ID_Formulario = id,
                    ID_Pregunta = idPregunta,
                    Orden = 1 // Puedes ajustar el orden si lo necesitas
                });
            }
        }
        await _context.SaveChangesAsync();
        return RedirectToAction("Index", "Formularios");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Desasignar(int formularioId, int preguntaId)
    {
        var relacion = await _context.FormularioPreguntas
            .FirstOrDefaultAsync(fp => fp.ID_Formulario == formularioId && fp.ID_Pregunta == preguntaId);

        if (relacion != null)
        {
            _context.FormularioPreguntas.Remove(relacion);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Details", "Formularios", new { id = formularioId });
    }
}