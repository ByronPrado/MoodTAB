using Microsoft.AspNetCore.Mvc;
using WebConTablas.Models;
using System.Threading.Tasks;

public class FormulariosAsignadosController : Controller
{
    private readonly IFormularioAsignadoService _asignadoService;
    private readonly IVistaDatosService _vistaDatosService;

    public FormulariosAsignadosController(IFormularioAsignadoService asignadoService, IVistaDatosService vistaDatosService)
    {
        _asignadoService = asignadoService;
        _vistaDatosService = vistaDatosService;
    }

    public async Task<IActionResult> Index()
        => View(await _asignadoService.ObtenerTodosAsync());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _asignadoService.EliminarAsync(id);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Asignar()
    {
        ViewBag.Formularios = await _vistaDatosService.ObtenerFormulariosAsync();
        ViewBag.Pacientes = await _vistaDatosService.ObtenerPacientesAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Asignar(int ID_Formulario, int ID_Paciente, DateTime? Fecha_Limite)
    {
        await _asignadoService.AsignarFormularioAsync(ID_Formulario, ID_Paciente, Fecha_Limite);
        return RedirectToAction(nameof(Index));
    }
}
