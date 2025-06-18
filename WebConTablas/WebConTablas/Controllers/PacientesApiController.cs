using Microsoft.AspNetCore.Mvc;
using WebConTablas.Models;

[ApiController]
[Route("api/pacientes")]
public class PacientesApiController : ControllerBase
{
    private readonly AppDbContext _context;
    public PacientesApiController(AppDbContext context) => _context = context;

    [HttpGet]
    public string GetPacientes()
    {
        return "Conexión exitosa a la Web";
    }

    [HttpGet]
    public IActionResult GetPreguntas()
    {
        return Ok(_context.Pregunta.ToList());
    }
}