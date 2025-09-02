using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebConTablas.Models;

namespace WebConTablas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiComentariosExternosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ApiComentariosExternosController(AppDbContext context)
        {
            _context = context;
        }
        public class ComentarioRequest
        {
            public int IdUsuarioExterno { get; set; }
            public string Comentario { get; set; }
            public DateTime Fecha { get; set; }
        }

        // POST: api/ComentariosExternosApi
        [HttpPost]
        public async Task<IActionResult> PostComentario([FromBody] ComentarioRequest request)
        {
            if (!await _context.UsuariosExternos.AnyAsync(ue => ue.IdUsuarioExterno == request.IdUsuarioExterno))
            {
                return BadRequest(new { message = "El usuario externo especificado no existe." });
            }

            request.Fecha = DateTime.UtcNow;
            var comentario = new ComentariosExternos
            {
                IdUsuarioExterno = request.IdUsuarioExterno,
                Comentario = request.Comentario,
                Fecha = DateTime.UtcNow
            };
            _context.ComentariosExternos.Add(comentario);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Comentario creado exitosamente" });
        }
    }
}
