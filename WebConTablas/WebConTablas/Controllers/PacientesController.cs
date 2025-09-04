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
        .Include(p => p.UsuariosExternos)
            .ThenInclude(ue => ue.Comentarios)
        .ToListAsync();

        // --- Lógica de alerta por estado exaltado/inhibido 3 días o más ---
        var alertas = new List<string>();
        foreach (var paciente in pacientes)
        {
            // Toma los últimos 7 diarios emocionales (o los que haya)
            var ultimosDiarios = paciente.DiariosEmocionales
                .OrderByDescending(d => d.Fecha)
                .Take(7)
                .ToList();

            // Cuenta días con estado "exaltado" o "inhibido"
            int diasAlteradosIn = ultimosDiarios
                .Count(d => d.Estado == "inhibido");

            int diasAlteradosEx = ultimosDiarios
                .Count(d => d.Estado == "exaltado");

            if (ultimosDiarios.Count() >= 3) {

                // Promedios
                var suma_pasos = 0;
                var suma_celular = 0;
                foreach (var diarios in ultimosDiarios.Take(ultimosDiarios.Count - 1))
                {
                    suma_pasos += diarios.Pasos ?? 0;
                    suma_celular += diarios.Horas_celular ?? 0;
                }
                var promedio_pasos = suma_pasos / (ultimosDiarios.Count() -1);
                var promedio_celular = suma_celular / (ultimosDiarios.Count() -1);
                // margen de diferencia
                var margen_pasos = 10000;
                var margen_celular = 10;

                if (margen_pasos + promedio_pasos < ultimosDiarios.Last().Pasos || promedio_pasos - margen_pasos > ultimosDiarios.Last().Pasos)
                {
                    alertas.Add($"El paciente <b>{paciente.Nombre}</b> tiene una alteración inusual en sus <b>pasos</b>.     <a href=\"/Pacientes/Details/{paciente.ID_Paciente}/#listadiarios\" class=\"text-primary text-decoration-none\">Ver Detalles</a>");
                }

                if (margen_celular + promedio_celular < ultimosDiarios.Last().Horas_celular || promedio_celular - margen_celular > ultimosDiarios.Last().Horas_celular)
                {
                    alertas.Add($"El paciente <b>{paciente.Nombre}</b> tiene una alteración inusual en sus <b>Horas de celular</b>.     <a href=\"/Pacientes/Details/{paciente.ID_Paciente}/#listadiarios\" class=\"text-primary text-decoration-none\">Ver Detalles</a>");
                }
            }
            



            if (diasAlteradosIn >= 2)
            {
                alertas.Add($"El paciente <b>{paciente.Nombre}</b> lleva {diasAlteradosIn} días en estado <b>inhibido</b>.     <a href=\"/Pacientes/Details/{paciente.ID_Paciente}/#listadiarios\" class=\"text-primary text-decoration-none\">Ver Detalles</a>");
            }

            if (diasAlteradosEx >= 2)
            {
                alertas.Add($"El paciente <b>{paciente.Nombre}</b> lleva {diasAlteradosEx} días en estado <b>exaltado</b>.     <a href=\"/Pacientes/Details/{paciente.ID_Paciente}/#listadiarios\" class=\"text-primary text-decoration-none\">Ver Detalles</a>");
            }
        }
        ViewBag.Alertas = alertas;

        

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
