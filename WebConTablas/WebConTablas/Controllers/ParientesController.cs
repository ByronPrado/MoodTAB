using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebConTablas.Models;
using Microsoft.EntityFrameworkCore;

namespace WebConTablas.Controllers
{
    public class ParientesController : Controller
    {
        private readonly AppDbContext _context;

        public ParientesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Parientes/
        public async Task<IActionResult> Index()
        {
            var parientes = await _context.UsuariosExternos.Include(u => u.Paciente).ToListAsync();
            return View(parientes);
        }

        // GET: /Parientes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Parientes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioExterno usuarioExterno)
        {
            if (ModelState.IsValid)
            {
                _context.UsuariosExternos.Add(usuarioExterno);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(usuarioExterno);
        }
    }
}
