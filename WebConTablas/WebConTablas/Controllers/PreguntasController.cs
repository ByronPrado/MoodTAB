using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

public class PreguntasController : Controller
{
    private readonly AppDbContext _context;

    public PreguntasController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Create(int idPaciente)
    {
        var pregunta = new Pregunta { IdPaciente = idPaciente };
        return View(pregunta);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int idPaciente)
    {
        var paciente = await _context.Pacientes
            .Include(p => p.Preguntas)
            .FirstOrDefaultAsync(p => p.Id == idPaciente);

        if (paciente == null)
            return NotFound();

        return View(paciente);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Pregunta pregunta)
    {
        Console.WriteLine("Entrando al POST de Pregunta con contenido: " + pregunta.Contenido);

        if (ModelState.IsValid)
        {
            _context.Preguntas.Add(pregunta);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Pacientes");
        }

        if (!ModelState.IsValid)
        {
            foreach (var key in ModelState.Keys)
            {
                var state = ModelState[key];
                foreach (var error in state.Errors)
                {
                    Console.WriteLine($"Error en {key}: {error.ErrorMessage}");
                }
            }
        }


        Console.WriteLine("ModelState inválido.");
        return View(pregunta);
    }

    public async Task<IActionResult> UpdatePregunta(int Id, string Contenido, int IdPaciente)
    {
        var pregunta = await _context.Preguntas.FindAsync(Id);
        if (pregunta == null)
            return NotFound();

        pregunta.Contenido = Contenido;
        _context.Preguntas.Update(pregunta);
        await _context.SaveChangesAsync();

        return RedirectToAction("Edit", new { idPaciente = IdPaciente });
    }

    public async Task<IActionResult> DeletePregunta(int Id, int IdPaciente)
    {
        var pregunta = await _context.Preguntas.FindAsync(Id);
        if (pregunta == null)
            return NotFound();

        _context.Preguntas.Remove(pregunta);
        await _context.SaveChangesAsync();

        return RedirectToAction("Edit", new { idPaciente = IdPaciente });
    }

}