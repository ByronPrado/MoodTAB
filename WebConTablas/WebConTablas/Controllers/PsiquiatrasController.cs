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
    public async Task<IActionResult> Details()
    {
        // Usamos la Sesión
        int? psiquiatraId = HttpContext.Session.GetInt32("PsiquiatraId");

        // Si no hay ID en la sesión, lo mandamos al login.
        if (psiquiatraId == null || psiquiatraId == 0)
        {
            return RedirectToAction("Login", "Psiquiatras");
        }

        // Buscamos al psiquiatra y cargamos SUS recordatorios
        var psiquiatra = await _context.Psiquiatras 
            .Include(p => p.RecordatorioPsiquiatra) 
            .FirstOrDefaultAsync(p => p.ID_Psiquiatra == psiquiatraId);

        if (psiquiatra == null)
        {
            return NotFound("No se encontró el perfil del psiquiatra.");
        }

        // 3. Pasamos el modelo completo a la vista
        // Esto buscará la vista en /Views/Psiquiatras/Details.cshtml
        return View(psiquiatra);
    }

    // GET: Psiquiatras/Login
    public IActionResult Login()
    {
        return View();
    }

    // POST: Psiquiatras/Login
    [HttpPost]
    public IActionResult Login(string usuario, string contrasena)
    {
        if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(contrasena))
        {
            ViewBag.Error = "Debes ingresar usuario y contraseña.";
            return View();
        }

        var usuarioInput = usuario.Trim().ToLower();
        var contrasenaInput = contrasena.Trim();

        var psiquiatra = _context.Psiquiatras
        .FirstOrDefault(p =>
            (p.Nombre != null && p.Nombre.Trim().ToLower() == usuarioInput ||
             p.Email != null && p.Email.Trim().ToLower() == usuarioInput)
            && (p.Contrasena != null && p.Contrasena.Trim() == contrasenaInput)
        );

        if (psiquiatra != null)
        {
            HttpContext.Session.SetInt32("PsiquiatraId", psiquiatra.ID_Psiquiatra);
            return RedirectToAction("Details", "Psiquiatras");
        }
        else
        {
            ViewBag.Error = "Usuario o contraseña incorrectos.";
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
        return RedirectToAction("Details", "Psiquiatra");
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