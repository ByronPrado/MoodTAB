using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebConTablas.Models;

[ApiController]
[Route("api/[controller]")]
public class AutenticacionLogin : ControllerBase
{
    private readonly AppDbContext _context;
    public AutenticacionLogin(AppDbContext context) => _context = context;

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        Console.WriteLine($"Request recibido: Nombre={request.Nombre}, Email={request.Email}");

        var user = _context.Pacientes
            .FirstOrDefault(u => u.Nombre == request.Nombre && u.Email == request.Email);

        if (user == null)
        {
            Console.WriteLine("Usuario no encontrado");
            return Unauthorized(new { Success = false, Message = "Credenciales inválidas" });
        }

        Console.WriteLine($"Usuario encontrado: {user.Nombre}, {user.Email}");
        return Ok(new
        {
            Success = true,
            User = new { user.ID_Paciente, user.Nombre, user.Email }
        });
    }
}