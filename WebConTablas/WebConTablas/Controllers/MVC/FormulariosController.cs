using Microsoft.AspNetCore.Mvc;
using WebConTablas.Models;

public class FormulariosController : Controller
{
    private readonly IFormularioService _formularioService;
    private readonly IVistaDatosService _vistaDatos;

    public FormulariosController(IFormularioService formularioService, IVistaDatosService vistaDatos)
    {
        _formularioService = formularioService;
        _vistaDatos = vistaDatos;
    }

    public async Task<IActionResult> Index()
        => View(await _formularioService.ObtenerTodosAsync());

    public async Task<IActionResult> Create()
    {
        await CargarPsiquiatrasAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Formulario formulario)
    {
        if (!ModelState.IsValid)
        {
            MostrarErroresValidacion();
            await CargarPsiquiatrasAsync();
            return View(formulario);
        }

        await _formularioService.CrearAsync(formulario);
        return RedirectToAction("Asignar", "FormularioPreguntas", new { id = formulario.ID_Formulario });
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var formulario = await _formularioService.ObtenerDetallesAsync(id.Value);
        if (formulario == null) return NotFound();

        return View(formulario);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var formulario = await _formularioService.ObtenerDetallesAsync(id.Value);
        if (formulario == null) return NotFound();

        await CargarPsiquiatrasAsync();
        return View(formulario);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Formulario formulario)
    {
        if (id != formulario.ID_Formulario) return NotFound();

        if (!ModelState.IsValid)
        {
            await CargarPsiquiatrasAsync();
            return View(formulario);
        }

        await _formularioService.EditarAsync(formulario);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var formulario = await _formularioService.ObtenerDetallesAsync(id.Value);
        if (formulario == null) return NotFound();

        return View(formulario);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _formularioService.EliminarAsync(id);
        return RedirectToAction(nameof(Index));
    }

    // Helpers
    private async Task CargarPsiquiatrasAsync()
    {
        ViewBag.Psiquiatras = await _vistaDatos.ObtenerPsiquiatrasAsync();
    }

    private void MostrarErroresValidacion()
    {
        foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
        {
            Console.WriteLine("Error de validación: " + error.ErrorMessage);
        }
    }
}
