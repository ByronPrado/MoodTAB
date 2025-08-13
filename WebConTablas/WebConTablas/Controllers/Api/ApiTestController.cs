using Microsoft.AspNetCore.Mvc;
using WebConTablas.Models;

[ApiController]
[Route("api/pacientes")]
public class ApiTestController : ControllerBase
{
    private readonly AppDbContext _context;
    public ApiTestController(AppDbContext context) => _context = context;

    [HttpGet]
    public string GetPacientes()
    {
        return "Conexión exitosa a la Web";
    }
    
}
   