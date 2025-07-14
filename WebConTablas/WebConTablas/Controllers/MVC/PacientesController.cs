using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebConTablas.Services;
using WebConTablas.Models;
using Microsoft.AspNetCore.Http;

[RequiereSesionPsiquiatra]
public class PacientesController : Controller
{
    private readonly IPacienteService _pacienteService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PacientesController(IPacienteService pacienteService, IHttpContextAccessor httpContextAccessor)
    {
        _pacienteService = pacienteService;
        _httpContextAccessor = httpContextAccessor;
    }

    private int? ObtenerIdPsiquiatraSesion() =>
        ControllerHelper.ObtenerIdPsiquiatraSesionConValidacion(this, _httpContextAccessor.HttpContext!);

    public async Task<IActionResult> Index()
    {
        var idPsiquiatra = ObtenerIdPsiquiatraSesion();
        if (idPsiquiatra == null)
            return RedirectHelper.RedirigirLogin(this);

        var pacientes = await _pacienteService.ObtenerPacientesPorPsiquiatraAsync(idPsiquiatra.Value);
        return View(pacientes);
    }

    public IActionResult Create() => View();

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var idPsiquiatra = ObtenerIdPsiquiatraSesion();
        if (idPsiquiatra == null)
            return RedirectHelper.RedirigirLogin(this);

        var paciente = await _pacienteService.ObtenerPacientePorIdAsync(id, idPsiquiatra.Value);
        if (paciente == null)
            return NotFound();

        return View(paciente);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Paciente paciente)
    {
        var idPsiquiatra = ObtenerIdPsiquiatraSesion();
        if (idPsiquiatra == null)
            return RedirectHelper.RedirigirLogin(this);

        paciente.ID_Psiquiatra = idPsiquiatra.Value;

        if (!ControllerHelper.ValidarModeloYMostrarErrores(ModelState, this))
            return View(paciente);

        await _pacienteService.CrearPacienteAsync(paciente);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Paciente paciente)
    {
        var idPsiquiatra = ObtenerIdPsiquiatraSesion();
        if (idPsiquiatra == null)
            return RedirectHelper.RedirigirLogin(this);

        var pacienteExistente = await _pacienteService.ObtenerPacientePorIdAsync(paciente.ID_Paciente, idPsiquiatra.Value);
        if (pacienteExistente == null)
            return Unauthorized();

        if (!ControllerHelper.ValidarModeloYMostrarErrores(ModelState, this))
            return View(paciente);

        // Actualiza campos
        pacienteExistente.Nombre = paciente.Nombre;
        pacienteExistente.Diagnostico = paciente.Diagnostico;
        pacienteExistente.Edad = paciente.Edad;
        pacienteExistente.Sexo = paciente.Sexo;
        pacienteExistente.Email = paciente.Email;
        pacienteExistente.Telefono = paciente.Telefono;

        await _pacienteService.EditarPacienteAsync(pacienteExistente);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var idPsiquiatra = ObtenerIdPsiquiatraSesion();
        if (idPsiquiatra == null)
            return RedirectHelper.RedirigirLogin(this);

        await _pacienteService.EliminarPacienteAsync(id, idPsiquiatra.Value);
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Details(int id)
    {
        var idPsiquiatra = ObtenerIdPsiquiatraSesion();
        if (idPsiquiatra == null)
            return RedirectToAction("Login", "Psiquiatras");

        // Aquí usa tu servicio para obtener el paciente por id y psiquiatra
        var paciente = await _pacienteService.ObtenerPacientePorIdAsync(id, idPsiquiatra.Value);

        if (paciente == null)
            return NotFound();

        // Asegúrate que el método ObtenerPacientePorIdAsync incluya las relaciones necesarias,
        // como DiariosEmocionales, FormulariosAsignados, etc.
        return View(paciente);
    }

}
