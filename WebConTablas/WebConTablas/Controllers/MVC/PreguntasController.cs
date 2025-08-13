using Microsoft.AspNetCore.Mvc;
using WebConTablas.Models;

public class PreguntasController : Controller
{
    private readonly IPreguntaService _preguntaService;

    public PreguntasController(IPreguntaService preguntaService)
    {
        _preguntaService = preguntaService;
    }

    public async Task<IActionResult> Index()
        => View(await _preguntaService.ObtenerTodasAsync());

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Pregunta pregunta)
    {
        if (!ControllerHelper.ValidarModeloYMostrarErrores(ModelState, this))
        {
            return View(pregunta);
        }

        pregunta.Created_at = DateTime.UtcNow;
        await _preguntaService.CrearAsync(pregunta);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var pregunta = await _preguntaService.ObtenerPorIdAsync(id.Value);
        if (pregunta == null) return NotFound();

        return View(pregunta);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Pregunta pregunta)
    {
        if (id != pregunta.ID_Pregunta) return NotFound();

        if (!ControllerHelper.ValidarModeloYMostrarErrores(ModelState, this))
        {
            return View(pregunta);
        }

        await _preguntaService.EditarAsync(pregunta);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var pregunta = await _preguntaService.ObtenerPorIdAsync(id.Value);
        if (pregunta == null) return NotFound();

        return View(pregunta);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _preguntaService.EliminarAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
