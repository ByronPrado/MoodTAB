using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebConTablas.Models;
using System;

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

    // Borrar asignaciones.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var asignacion = await _context.FormulariosAsignados.FindAsync(id);
        if (asignacion != null)
        {
            _context.FormulariosAsignados.Remove(asignacion);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
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
    public async Task<IActionResult> Asignar(List<int> ID_Formulario, int ID_Paciente, DateTime? Fecha_Limite)
    {
        var errores = new List<string>();

        foreach (var formularioId in ID_Formulario)
        {
            // Verificar si ya existe asignacion previa
            bool yaExiste = await _context.FormulariosAsignados
                .AnyAsync(fa => fa.ID_Formulario == formularioId && fa.ID_Paciente == ID_Paciente);

            if (yaExiste)
            {
                var titulo = await _context.Formularios
                    .Where(f => f.ID_Formulario == formularioId)
                    .Select(f => f.Titulo)
                    .FirstOrDefaultAsync();

                errores.Add($"El formulario \"{titulo}\" ya fue asignado a este paciente.");
                continue; // se omite el duplicado
            }

            // Crear la nueva asignación
            var asignacion = new FormularioAsignado
            {
                ID_Formulario = formularioId,
                ID_Paciente = ID_Paciente,
                Fecha_Asignacion = DateTime.UtcNow,
                Fecha_Limite = Fecha_Limite?.ToUniversalTime(),
                Estado = "pendiente"
            };

            _context.FormulariosAsignados.Add(asignacion);
        }

        // En caso de errores, avisar.
        if (errores.Any())
        {
            ViewBag.Formularios = _context.Formularios.ToList();
            ViewBag.Pacientes = _context.Pacientes.ToList();
            ViewBag.Errores = errores;
            return View();
        }

        // Guardar cambios en caso contrario.
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
