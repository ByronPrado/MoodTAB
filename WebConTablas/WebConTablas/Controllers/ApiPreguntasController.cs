using Microsoft.AspNetCore.Mvc;
using WebConTablas.Models;

[ApiController]
[Route("api/preguntas")]
public class ApiPreguntaController : ControllerBase
{
    private readonly AppDbContext _context;
    public ApiPreguntaController(AppDbContext context) => _context = context;

    [HttpGet]
    public IActionResult GetPreguntas()
    {
        return Ok(_context.Preguntas.ToList());
    }
}