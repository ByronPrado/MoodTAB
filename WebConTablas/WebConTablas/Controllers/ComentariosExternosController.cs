using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebConTablas.Models;

namespace WebConTablas.Controllers
{
    public class ComentariosExternosController : Controller
    {
        private readonly AppDbContext _context;

        public ComentariosExternosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /ComentariosExternos/Create
        public IActionResult Create()
        {
            return View(new ComentariosExternos());
        }

        // POST: /ComentariosExternos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ComentariosExternos comentario)
        {
            Console.WriteLine($"[DEBUG] IdUsuarioExterno recibido: {comentario.IdUsuarioExterno}");
            ViewBag.Recibido = comentario.IdUsuarioExterno;
            if (!await _context.UsuariosExternos.AnyAsync(ue => ue.IdUsuarioExterno == comentario.IdUsuarioExterno))
            {
                ModelState.AddModelError("IdUsuarioExterno", "El usuario externo especificado no existe.");
            }
            if (ModelState.IsValid)
            {
                comentario.Fecha = System.DateTime.UtcNow;
                _context.ComentariosExternos.Add(comentario);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Parientes");
            }
            return View(comentario);
        }
    }
}
