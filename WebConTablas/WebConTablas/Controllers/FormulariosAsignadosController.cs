using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebConTablas.Models;

public class FormulariosAsignadosController : Controller
{
    private readonly AppDbContext _context;
    public FormulariosAsignadosController(AppDbContext context) => _context = context;

    // Listado de asignaciones
    public async Task<IActionResult> Index()
    {
        var asignaciones = await _context.FormulariosAsignados
            .Include(fa => fa.Formulario)
            .Include(fa => fa.Paciente)
            .ToListAsync();
        return View(asignaciones);
    }

    // GET: FormulariosAsignados/Asignar
    public IActionResult Asignar()
    {
        ViewBag.Formularios = _context.Formularios.ToList();
        ViewBag.Pacientes = _context.Pacientes.ToList();
        return View();
    }

    // POST: FormulariosAsignados/Asignar
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Asignar(int ID_Formulario, int ID_Paciente, DateTime? Fecha_Limite)
    {
        var asignacion = new FormularioAsignado
        {
            ID_Formulario = ID_Formulario,
            ID_Paciente = ID_Paciente,
            Fecha_Asignacion = DateTime.Now,
            Fecha_Limite = Fecha_Limite,
            Estado = "pendiente"
        };
        _context.FormulariosAsignados.Add(asignacion);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}