using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebConTablas.Models;
using System.Linq;

public class PsiquiatrasController : Controller
{
    private readonly AppDbContext _context;

    public PsiquiatrasController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var psiquiatras = await _context.Psiquiatras.ToListAsync();
        return View(psiquiatras);
    }

    // GET: Psiquiatras/Login
    public IActionResult Login()
    {
        return View();
    }

    // POST: Psiquiatras/Login
    [HttpPost]
    public IActionResult Login(Psiquiatras model)
    {
        var psiquiatra = _context.Psiquiatras
            .FirstOrDefault(p => p.Name == model.Name && p.Correo == model.Correo);

        if (psiquiatra != null)
        {
            // Autenticación exitosa: redirigir a otra vista (ej. Index)
            HttpContext.Session.SetInt32("PsiquiatraId", psiquiatra.Id);
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
    public async Task<IActionResult> Create(Psiquiatras psiquiatras)
    {
        _context.Psiquiatras.Add(psiquiatras);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var psiquiatras = await _context.Psiquiatras.FindAsync(id);
        if (psiquiatras == null) return NotFound();
        return View(psiquiatras);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Psiquiatras psiquiatras)
    {
        _context.Psiquiatras.Update(psiquiatras);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var psiquiatras = await _context.Psiquiatras.FindAsync(id);
        if (psiquiatras == null) return NotFound();
        _context.Psiquiatras.Remove(psiquiatras);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
