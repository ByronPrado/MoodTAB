using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

public class ClientController : Controller
{
    private readonly AppDbContext _context;

    public ClientController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var clients = await _context.Clients.ToListAsync();
        return View(clients);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Client client)
    {
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client == null) return NotFound();
        return View(client);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Client client)
    {
        _context.Clients.Update(client);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client == null) return NotFound();
        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
