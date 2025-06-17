using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebConTablas.Models;

public class PreguntasController : Controller
{
    private readonly AppDbContext _context;
    public PreguntasController(AppDbContext context) => _context = context;

    public async Task<IActionResult> Index() => View(await _context.Preguntas.ToListAsync());

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Pregunta pregunta)
    {
        if (ModelState.IsValid)
        {
            pregunta.Created_at = DateTime.Now;
            _context.Add(pregunta);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(pregunta);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var pregunta = await _context.Preguntas.FindAsync(id);
        if (pregunta == null) return NotFound();
        return View(pregunta);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Pregunta pregunta)
    {
        if (id != pregunta.ID_Pregunta) return NotFound();
        if (ModelState.IsValid)
        {
            _context.Update(pregunta);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(pregunta);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var pregunta = await _context.Preguntas.FindAsync(id);
        if (pregunta == null) return NotFound();
        return View(pregunta);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var pregunta = await _context.Preguntas.FindAsync(id);
        if (pregunta != null)
        {
            _context.Preguntas.Remove(pregunta);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}