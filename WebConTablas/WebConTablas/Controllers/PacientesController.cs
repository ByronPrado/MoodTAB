using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebConTablas.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.Rendering;

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
        .Include(p => p.Alertas)
        .ToListAsync();

        var alertasNoVistas = await _context.Alertas
            .Include(a => a.Paciente)
            .Where(a => a.Paciente != null && a.Paciente.ID_Psiquiatra == idPsiquiatra && a.Estado == "No Visto")
            .OrderByDescending(a => a.Created_at)
            .ToListAsync() ?? new List<Alertas>();

        ViewBag.AlertasNoVistas = alertasNoVistas;

        return View(pacientes);
    }

    [HttpPost]
    public async Task<IActionResult> MarcarAlertaVista(int idAlerta)
    {
        var alerta = await _context.Alertas.FindAsync(idAlerta);
        if (alerta != null)
        {
            alerta.Estado = "Visto";
            await _context.SaveChangesAsync();
        }
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> ActualizarEstadoAlerta([FromBody] AlertaUpdateRequest request)
    {
        try
        {
            var alerta = await _context.Alertas.FindAsync(request.IdAlerta);
            if (alerta != null)
            {
                alerta.Estado = request.Estado;
                await _context.SaveChangesAsync();
                return Ok(new { success = true });
            }
            return NotFound(new { success = false, message = "Alerta no encontrada" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    public class AlertaUpdateRequest
    {
        public int IdAlerta { get; set; }
        public string? Estado { get; set; }
    }

    public IActionResult Create()
    {
        var idPsiquiatra = HttpContext.Session.GetInt32("PsiquiatraId");

        if (idPsiquiatra == null)
        {
            return RedirectToAction("Login", "Psiquiatras");
        }

        // Filtrar solo los formularios del psiquiatra y del grupo "Formulario de Autoevaluación"
        var formularios = _context.Formularios
            .Where(f => f.ID_Psiquiatra == idPsiquiatra && f.Grupo == "Formulario de Autoevaluación")
            .Select(f => new { f.ID_Formulario, f.Titulo })
            .ToList();

        ViewBag.FormulariosAutoevaluacion = new SelectList(formularios, "ID_Formulario", "Titulo");

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
    public async Task<IActionResult> Create(Paciente paciente, int? ID_FormularioSeleccionado)
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

            // Si seleccionó un formulario, se le asigna al paciente recién creado
            if (ID_FormularioSeleccionado.HasValue)
            {
                var asignacion = new FormularioAsignado
                {
                    ID_Formulario = ID_FormularioSeleccionado.Value,
                    ID_Paciente = paciente.ID_Paciente,
                    Fecha_Asignacion = DateTime.UtcNow
                };

                _context.Add(asignacion);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // Si falla la validación, volvemos a cargar el combo de formularios
        var formularios = _context.Formularios
            .Where(f => f.ID_Psiquiatra == idPsiquiatra && f.Grupo == "Formulario de Autoevaluación")
            .Select(f => new { f.ID_Formulario, f.Titulo })
            .ToList();

        ViewBag.FormulariosAutoevaluacion = new SelectList(formularios, "ID_Formulario", "Titulo");

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
