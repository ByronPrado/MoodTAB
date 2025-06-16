using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

public class DiarioPacienteController : Controller
{
    private readonly AppDbContext _context;

    public DiarioPacienteController(AppDbContext context)
    {
        _context = context;
    }

    // Mostrar diarios de un paciente
    public async Task<IActionResult> Index(int idPaciente)
    {
        var diarios = await _context.DiariosPacientes
            .Where(d => d.IdPaciente == idPaciente)
            .OrderByDescending(d => d.Fecha)
            .ToListAsync();

        ViewBag.IdPaciente = idPaciente;
        return View(diarios);
    }

    // Formulario para crear un diario
    public IActionResult Create(int idPaciente)
    {
        var diario = new DiarioPaciente { IdPaciente = idPaciente, Fecha = DateTime.Today };
        return View(diario);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DiarioPaciente diario)
    {
        if (ModelState.IsValid)
        {
            _context.Add(diario);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { idPaciente = diario.IdPaciente });
        }
        return View(diario);
    }
}