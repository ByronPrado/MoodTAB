using Microsoft.AspNetCore.Mvc;
using WebConTablas.Models;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/formulario")]
public class ApiFormularioController : ControllerBase
{
    private readonly AppDbContext _context;
    public ApiFormularioController(AppDbContext context) => _context = context;

    [HttpGet("{pacienteId}")]
    public IActionResult GetFormularioPorPaciente(int pacienteId)
    {
        var asignacion = _context.FormulariosAsignados
            .Where(fa => fa.ID_Paciente == pacienteId)
            .OrderByDescending(fa => fa.Fecha_Asignacion)
            .Select(fa => new
            {
                fa.ID_Asignacion,
                fa.Estado,
                fa.Fecha_Asignacion,
                fa.Fecha_Limite,
                Formulario = new
                {
                    fa.Formulario.ID_Formulario,
                    fa.Formulario.Titulo,
                    fa.Formulario.Descripcion,
                    Preguntas = fa.Formulario.Preguntas.Select(fp => new
                    {
                        fp.Pregunta.ID_Pregunta,
                        fp.Pregunta.Contenido,
                        fp.Pregunta.Tipo,
                        fp.Pregunta.Extra,
                        fp.Pregunta.OpcionesSeleccion,
                        fp.Pregunta.EscalaMin,
                        fp.Pregunta.EscalaMax
                    })
                }
            })
            .FirstOrDefault();

        if (asignacion == null)
            return NotFound();

        return Ok(asignacion);
    }
    [HttpPost("responder")]
    public async Task<IActionResult> GuardarRespuestas([FromBody] RespuestasFormularioDto dto)
    {
        var asignacion = await _context.FormulariosAsignados
            .Include(fa => fa.Respuestas)
            .FirstOrDefaultAsync(fa => fa.ID_Asignacion == dto.ID_Asignacion);

        if (asignacion == null)
            return NotFound();

        // Guardar cada respuesta
        foreach (var r in dto.Respuestas)
        {
            var respuesta = new Respuesta
            {
                ID_Asignacion = dto.ID_Asignacion,
                ID_Pregunta = r.ID_Pregunta,
                Contenido = r.Contenido,
                Fecha_Respuesta = DateTime.UtcNow
            };
            _context.Respuestas.Add(respuesta);
        }

        // Cambiar estado
        asignacion.Estado = "Listo";
        await _context.SaveChangesAsync();

        return Ok(new { success = true });
    }

    // DTOs
    public class RespuestasFormularioDto
    {
        public int ID_Asignacion { get; set; }
        public List<RespuestaDto> Respuestas { get; set; }
    }
    public class RespuestaDto
    {
        public int ID_Pregunta { get; set; }
        public string Contenido { get; set; }
    }
}