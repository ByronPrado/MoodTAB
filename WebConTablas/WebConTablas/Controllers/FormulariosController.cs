using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebConTablas.Models;

public class FormulariosController : Controller
{
    private readonly AppDbContext _context;
    public FormulariosController(AppDbContext context) => _context = context;

    public async Task<IActionResult> Index() => View(await _context.Formularios.Include(f => f.Psiquiatra).ToListAsync());

    public IActionResult Create()
    {
        ViewBag.Psiquiatras = _context.Psiquiatras.ToList();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Formulario formulario)
    {
        if (ModelState.IsValid)
        {
            formulario.Created_at = DateTime.Now;
            _context.Add(formulario);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Psiquiatras = _context.Psiquiatras.ToList();
        return View(formulario);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var formulario = await _context.Formularios.FindAsync(id);
        if (formulario == null) return NotFound();
        ViewBag.Psiquiatras = _context.Psiquiatras.ToList();
        return View(formulario);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Formulario formulario)
    {
        if (id != formulario.ID_Formulario) return NotFound();
        if (ModelState.IsValid)
        {
            _context.Update(formulario);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Psiquiatras = _context.Psiquiatras.ToList();
        return View(formulario);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var formulario = await _context.Formularios.FindAsync(id);
        if (formulario == null) return NotFound();
        return View(formulario);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var formulario = await _context.Formularios.FindAsync(id);
        if (formulario != null)
        {
            _context.Formularios.Remove(formulario);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}