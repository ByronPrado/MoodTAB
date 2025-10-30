using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebConTablas.Models;

namespace WebConTablas.Controllers
{
    public class RecordatoriosPsiquiatraController : Controller
    {
        private readonly AppDbContext _context;
        private string GetColorFromGroup(string? grupo)
        {
            return grupo switch
            {
                "Personal" => "#0d6efd",   // Azul (Bootstrap Primary)
                "Cita" => "#198754",    // Verde (Bootstrap Success)
                "Importante" => "#dc3545", // Rojo (Bootstrap Danger)
                "Reunión" => "#ffc107",  // Amarillo (Bootstrap Warning)
                _ => "#6c757d",           // Gris (Bootstrap Secondary)
            };
        }
        public RecordatoriosPsiquiatraController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> GuardarRecordatorio([FromBody] RecordatoriosPsiquiatra recordatorio)
        {
            // GET ID
            int? psiquiatraId = HttpContext.Session.GetInt32("PsiquiatraId");

            if (psiquiatraId == null)
            {
                return Unauthorized(); 
            }

            if (recordatorio.Fecha.HasValue)
            {
                if (recordatorio.IsAllDay)
                {
                    // Si es "Todo el día", la hora es 00:00. Solo la marcamos como UTC.
                    recordatorio.Fecha = DateTime.SpecifyKind(recordatorio.Fecha.Value, DateTimeKind.Utc);
                }
                else
                {
                    // Si tiene hora, asumimos que es LOCAL y la CONVERTIMOS a UTC para guardarla.
                    recordatorio.Fecha = recordatorio.Fecha.Value.ToUniversalTime();
                }
            }

            // Forzamos que el recordatorio pertenezca al psiquiatra logeado
            recordatorio.ID_Psiquiatra = psiquiatraId.Value;

            if (ModelState.IsValid)
            {
                try
                {
                    if (recordatorio.ID_RecordatorioPsiquiatra == 0)
                    {
                        // CREAR
                        // Limpiamos la navegación para evitar problemas
                        recordatorio.Psiquiatra = null; 
                        _context.RecordatoriosPsiquiatra.Add(recordatorio);
                    }
                    else
                    {
                        // ACTUALIZAR
                        // 1. Buscamos el recordatorio ORIGINAL en la BBDD
                        var recordatorioDB = await _context.RecordatoriosPsiquiatra
                            .FirstOrDefaultAsync(r => r.ID_RecordatorioPsiquiatra == recordatorio.ID_RecordatorioPsiquiatra);

                        if (recordatorioDB == null)
                        {
                            return NotFound();
                        }
                        
                        // 2. Verificamos que le pertenece a este psiquiatra
                        if (recordatorioDB.ID_Psiquiatra != psiquiatraId.Value)
                        {
                            return Forbid(); // No tiene permiso
                        }

                        // 3. Actualizamos SÓLO las propiedades que vienen del modal
                        recordatorioDB.Titulo = recordatorio.Titulo;
                        recordatorioDB.Fecha = recordatorio.Fecha;
                        recordatorioDB.Descripcion = recordatorio.Descripcion;
                        recordatorioDB.Grupo = recordatorio.Grupo;
                        recordatorioDB.IsAllDay = recordatorio.IsAllDay;
                        recordatorioDB.Color = recordatorio.Color;
                        
                    }

                    await _context.SaveChangesAsync();

                    var color = !string.IsNullOrEmpty(recordatorio.Color) 
                        ? recordatorio.Color 
                        : GetColorFromGroup(recordatorio.Grupo);
                    
                    // Devolvemos el objeto en el formato que espera FullCalendar
                    var eventData = new
                    {
                        id = recordatorio.ID_RecordatorioPsiquiatra,
                        title = recordatorio.Titulo,
                        start = recordatorio.Fecha?.ToString("o"),
                        allDay = recordatorio.IsAllDay,
                        backgroundColor = color, 
                        borderColor = color,     
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


        [HttpPost] 
        public async Task<IActionResult> EliminarRecordatorio(int id)
        {
            int? psiquiatraId = HttpContext.Session.GetInt32("PsiquiatraId");

            if (psiquiatraId == null)
            {
                return Unauthorized();
            }

            var recordatorio = await _context.RecordatoriosPsiquiatra
                .FirstOrDefaultAsync(r => r.ID_RecordatorioPsiquiatra == id);

            if (recordatorio == null)
            {
                return NotFound();
            }

            // Verificación de seguridad:
            if (recordatorio.ID_Psiquiatra != psiquiatraId.Value) // Con .Value
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