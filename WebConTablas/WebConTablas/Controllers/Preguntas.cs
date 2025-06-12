using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class PreguntasController : ControllerBase
{
    // Simulación de datos. Usa tu DbContext real aquí.
    private static List<string> preguntas = new() { "¿Cómo te sientes hoy?", "¿Qué te motiva?" };

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(preguntas);
    }
}