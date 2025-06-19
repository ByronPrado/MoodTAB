using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebConTablas.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.TagHelpers;

public class PacientesController : Controller
{
    private readonly AppDbContext _context;

    public PacientesController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var idPsiquiatra = HttpContext.Session.GetInt32("PsiquiatraId");

        if (idPsiquiatra == null)
        {
            return RedirectToAction("Login", "Psiquiatras");
        }

        var pacientes = await _context.Pacientes
        .Where(p => p.ID_Psiquiatra == idPsiquiatra)
        .Include(p => p.DiariosEmocionales)
        .Include(p => p.FormulariosAsignados)
            .ThenInclude(fa => fa.Formulario)
        .Include(p => p.FormulariosAsignados)
            .ThenInclude(fa => fa.Respuestas)
                .ThenInclude(r => r.Pregunta)
        .ToListAsync();

        return View(pacientes);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var paciente = await _context.Pacientes.FindAsync(id);
        if (paciente == null)
            return NotFound();
        return View(paciente);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Paciente paciente)
    {
        var idPsiquiatra = HttpContext.Session.GetInt32("PsiquiatraId");

        if (idPsiquiatra == null)
        {
            return RedirectToAction("Login", "Psiquiatras");
        }

        paciente.ID_Psiquiatra = idPsiquiatra.Value;

        if (ModelState.IsValid)
        {
            _context.Pacientes.Add(paciente);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(paciente);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Paciente paciente)
    {
        var idPsiquiatra = HttpContext.Session.GetInt32("PsiquiatraId");

        if (idPsiquiatra == null)
            return RedirectToAction("Login", "Psiquiatras");

        // Asegúrate que el paciente le pertenece a ese psiquiatra
        var pacienteExistente = await _context.Pacientes
            .FirstOrDefaultAsync(p => p.ID_Paciente == paciente.ID_Paciente && p.ID_Psiquiatra == idPsiquiatra);

        if (pacienteExistente == null)
            return Unauthorized(); // o NotFound()

        if (ModelState.IsValid)
        {
            pacienteExistente.Nombre = paciente.Nombre;
            pacienteExistente.Diagnostico = paciente.Diagnostico;
            pacienteExistente.Edad = paciente.Edad;
            pacienteExistente.Sexo = paciente.Sexo;
            pacienteExistente.Email = paciente.Email;
            pacienteExistente.Telefono = paciente.Telefono;
            _context.Pacientes.Update(pacienteExistente);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(paciente);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var idPsiquiatra = HttpContext.Session.GetInt32("PsiquiatraId");

        if (idPsiquiatra == null)
            return RedirectToAction("Login", "Psiquiatras");

        var paciente = await _context.Pacientes
            .FirstOrDefaultAsync(p => p.ID_Paciente == id && p.ID_Psiquiatra == idPsiquiatra);

        if (paciente == null)
            return NotFound();

        _context.Pacientes.Remove(paciente);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

        public async Task<IActionResult> Details(int id)
    {
        var idPsiquiatra = HttpContext.Session.GetInt32("PsiquiatraId");

        if (idPsiquiatra == null)
        {
            return RedirectToAction("Login", "Psiquiatras");
        }

        // Busca el paciente específico, asegurándote que pertenece al psiquiatra
        // e incluye sus diarios emocionales.
        var paciente = await _context.Pacientes
            .Where(p => p.ID_Paciente == id && p.ID_Psiquiatra == idPsiquiatra)
            .Include(p => p.DiariosEmocionales)
            .FirstOrDefaultAsync();

        if (paciente == null)
        {
            // Si no se encuentra el paciente (o no pertenece al psiquiatra),
            // retorna un error 404.
            return NotFound();
        }

        // Pasa el objeto paciente a la nueva vista "Details.cshtml"
        return View(paciente);
    }


}
