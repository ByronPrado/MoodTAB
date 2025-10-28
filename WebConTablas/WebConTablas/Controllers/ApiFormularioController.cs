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
            .Where(fa => fa.ID_Paciente == pacienteId && fa.Estado == "pendiente")
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
            //.FirstOrDefault();            
            .ToList();

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

    [HttpGet("completados/{pacienteId}")]
    public IActionResult GetFormulariosCompletados(int pacienteId)
    {
        var asignaciones = _context.FormulariosAsignados
            .Where(fa => fa.ID_Paciente == pacienteId && fa.Estado == "Listo")
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
                    // Incluye también las respuestas del paciente
                    Preguntas = fa.Formulario.Preguntas.Select(fp => new
                    {
                        fp.Pregunta.ID_Pregunta,
                        fp.Pregunta.Contenido,
                        fp.Pregunta.Tipo,
                        fp.Pregunta.Extra,
                        fp.Pregunta.OpcionesSeleccion,
                        fp.Pregunta.EscalaMin,
                        fp.Pregunta.EscalaMax,
                        // Obtenemos la respuesta si existe
                        Respuesta = fa.Respuestas
                            .Where(r => r.ID_Pregunta == fp.Pregunta.ID_Pregunta)
                            .Select(r => r.Contenido)
                            .FirstOrDefault()
                    })
                }
            })
            .ToList();

        if (asignaciones == null || asignaciones.Count == 0)
            return NotFound(new { message = "No hay formularios completados." });

        return Ok(asignaciones);
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