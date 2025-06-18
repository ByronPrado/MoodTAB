using Microsoft.AspNetCore.Mvc;
using WebConTablas.Models;

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
            .Select(fa => new {
                fa.ID_Asignacion,
                fa.Estado,
                fa.Fecha_Asignacion,
                fa.Fecha_Limite,
                Formulario = new {
                    fa.Formulario.ID_Formulario,
                    fa.Formulario.Titulo,
                    fa.Formulario.Descripcion,
                    Preguntas = fa.Formulario.Preguntas.Select(fp => new {
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
}