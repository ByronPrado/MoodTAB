using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebConTablas.Models;

public class PsiquiatrasController : Controller
{
    private readonly AppDbContext _context;
    public PsiquiatrasController(AppDbContext context) => _context = context;

    public async Task<IActionResult> Index()
    {
        var lista = await _context.Psiquiatras.ToListAsync();
        return View(lista);
    }

    // GET: Psiquiatras/Login
    public IActionResult Login()
    {
        return View();
    }

    // POST: Psiquiatras/Login
    [HttpPost]
    public IActionResult Login(Psiquiatra model)
    {
        var psiquiatra = _context.Psiquiatras
            .FirstOrDefault(p => p.Nombre == model.Nombre && p.Email == model.Email);

        if (psiquiatra != null)
        {
            // Autenticación exitosa: redirigir a otra vista (ej. Index)
            HttpContext.Session.SetInt32("PsiquiatraId", psiquiatra.ID_Psiquiatra);
            return RedirectToAction("Index", "Pacientes");
        }
        else
        {
            // Falla de login
            ViewBag.Error = "Nombre o correo incorrectos.";
            return View();
        }
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Psiquiatra psiquiatra)
    {
        _context.Psiquiatras.Add(psiquiatra);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var psiquiatra = await _context.Psiquiatras.FindAsync(id);
        if (psiquiatra == null) return NotFound();
        return View(psiquiatra);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Psiquiatra psiquiatra)
    {
        _context.Psiquiatras.Update(psiquiatra);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var psiquiatra = await _context.Psiquiatras.FindAsync(id);
        if (psiquiatra == null) return NotFound();
        _context.Psiquiatras.Remove(psiquiatra);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}