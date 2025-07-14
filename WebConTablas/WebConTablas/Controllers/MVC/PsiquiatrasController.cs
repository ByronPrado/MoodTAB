using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using WebConTablas.Models;

public class PsiquiatrasController : Controller
{
    private readonly IPsiquiatraService _psiquiatraService;

    public PsiquiatrasController(IPsiquiatraService psiquiatraService)
    {
        _psiquiatraService = psiquiatraService;
    }

    public async Task<IActionResult> Index()
        => View(await _psiquiatraService.ObtenerTodosAsync());

    public IActionResult Login() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Error = "Datos inválidos.";
            return View(model);
        }

        var psiquiatra = await _psiquiatraService.BuscarPorCredencialesAsync(model.Nombre, model.Email);

        if (psiquiatra != null)
        {
            HttpContext.Session.SetInt32("PsiquiatraId", psiquiatra.ID_Psiquiatra);
            return RedirectToAction("Index", "Pacientes");
        }

        ViewBag.Error = "Nombre o correo incorrectos.";
        return View(model);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Psiquiatra psiquiatra)
    {
        if (!ModelState.IsValid) return View(psiquiatra);

        await _psiquiatraService.CrearAsync(psiquiatra);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var psiquiatra = await _psiquiatraService.ObtenerPorIdAsync(id);
        return psiquiatra == null ? NotFound() : View(psiquiatra);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Psiquiatra psiquiatra)
    {
        if (!ModelState.IsValid) return View(psiquiatra);

        await _psiquiatraService.EditarAsync(psiquiatra);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        await _psiquiatraService.EliminarAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
