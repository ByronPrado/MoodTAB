using Microsoft.AspNetCore.Mvc;
using WebConTablas.Models;

public class FormularioPreguntasController : Controller
{
    private readonly IFormularioPreguntaService _formularioPreguntaService;

    public FormularioPreguntasController(IFormularioPreguntaService formularioPreguntaService)
    {
        _formularioPreguntaService = formularioPreguntaService;
    }

    // GET: FormularioPreguntas/Asignar/5
    public IActionResult Asignar(int id)
    {
        var formulario = _formularioPreguntaService.ObtenerFormularioConPreguntas(id);
        var preguntasDisponibles = _formularioPreguntaService.ObtenerPreguntasNoAsignadas(id);

        ViewBag.Formulario = formulario;
        ViewBag.Preguntas = preguntasDisponibles;

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Asignar(int id, int[] preguntasSeleccionadas)
    {
        await _formularioPreguntaService.AsignarPreguntasAsync(id, preguntasSeleccionadas);
        return RedirectToAction("Index", "Formularios");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Desasignar(int formularioId, int preguntaId)
    {
        await _formularioPreguntaService.DesasignarPreguntaAsync(formularioId, preguntaId);
        return RedirectToAction("Details", "Formularios", new { id = formularioId });
    }
}
