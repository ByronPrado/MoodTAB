using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebConTablas.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebConTablas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LogsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> PostLog([FromBody] Logs log)
        {
            if (log == null)
                return BadRequest("Log vacío");

            try
            {
                log.Fecha ??= DateTime.UtcNow;

                _context.Logs.Add(log);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Log guardado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al guardar el log", error = ex.Message });
            }
        }
    }
}
