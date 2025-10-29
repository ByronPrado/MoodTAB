using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebConTablas.Models;
// using System.Security.Claims; // <-- YA NO LO NECESITAMOS

namespace WebConTablas.Controllers
{
    // [Authorize] // <-- 1. ELIMINAMOS ESTO (es la causa de tu error)
    public class RecordatoriosPsiquiatraController : Controller
    {
        private readonly AppDbContext _context;

        public RecordatoriosPsiquiatraController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> GuardarRecordatorio([FromBody] RecordatoriosPsiquiatra recordatorio)
        {
            // --- 3. CAMBIAMOS LA FORMA DE OBTENER EL ID ---
            int? psiquiatraId = HttpContext.Session.GetInt32("PsiquiatraId");

            if (psiquiatraId == null)
            {
                return Unauthorized(); // No debería pasar si ya está en la página
            }

            // Forzamos que el recordatorio pertenezca al psiquiatra logeado
            recordatorio.ID_Psiquiatra = psiquiatraId.Value;
            // --- FIN DEL CAMBIO ---

            if (ModelState.IsValid)
            {
                try
                {
                    if (recordatorio.ID_RecordatorioPsiquiatra == 0)
                    {
                        _context.RecordatoriosPsiquiatra.Add(recordatorio);
                    }
                    else
                    {
                        var existe = await _context.RecordatoriosPsiquiatra
                            .AsNoTracking()
                            .AnyAsync(r => r.ID_RecordatorioPsiquiatra == recordatorio.ID_RecordatorioPsiquiatra && 
                                           r.ID_Psiquiatra == psiquiatraId.Value); // Usamos .Value
                        
                        if (!existe)
                        {
                            return Forbid(); 
                        }

                        _context.RecordatoriosPsiquiatra.Update(recordatorio);
                    }

                    await _context.SaveChangesAsync();

                    // Devolvemos el objeto en el formato que espera FullCalendar
                    var eventData = new
                    {
                        id = recordatorio.ID_RecordatorioPsiquiatra,
                        title = recordatorio.Titulo,
                        start = recordatorio.Fecha?.ToString("yyyy-MM-dd"),
                        allDay = true,
                        extendedProps = new
                        {
                            descripcion = recordatorio.Descripcion,
                            grupo = recordatorio.Grupo
                        }
                    };

                    return Json(eventData);
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = "Error al guardar: " + ex.Message });
                }
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Elimina un recordatorio.
        /// </summary>
        [HttpPost] 
        public async Task<IActionResult> EliminarRecordatorio(int id)
        {
            // --- 4. CAMBIAMOS LA FORMA DE OBTENER EL ID ---
            int? psiquiatraId = HttpContext.Session.GetInt32("PsiquiatraId");

            if (psiquiatraId == null)
            {
                return Unauthorized();
            }
            // --- FIN DEL CAMBIO ---

            var recordatorio = await _context.RecordatoriosPsiquiatra
                .FirstOrDefaultAsync(r => r.ID_RecordatorioPsiquiatra == id);

            if (recordatorio == null)
            {
                return NotFound();
            }

            // Verificación de seguridad:
            if (recordatorio.ID_Psiquiatra != psiquiatraId.Value) // Usamos .Value
            {
                return Forbid(); 
            }

            try
            {
                _context.RecordatoriosPsiquiatra.Remove(recordatorio);
                await _context.SaveChangesAsync();
                
                return Ok(); 
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error al eliminar: " + ex.Message });
            }
        }
    }
}