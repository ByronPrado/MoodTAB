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
        Console.WriteLine($"Request recibido: Email={request.Email}, EsFamiliar={request.EsFamiliar}");

        if (request.EsFamiliar)
        {
            // tabla UsuariosExternos
            var familiar = _context.UsuariosExternos
                .FirstOrDefault(f => f.Contrasena == request.Password && f.Email == request.Email);

            if (familiar == null)
            {
                Console.WriteLine("Familiar no encontrado");
                return Unauthorized(new { Success = false, Message = "Credenciales inválidas" });
            }

            Console.WriteLine($"Familiar encontrado: {familiar.Nombre}, {familiar.Email}");
            return Ok(new
            {
                Success = true,
                TipoUsuario = $"familiar P: {familiar.Parentezco}", // puedes marcar de qué tipo es
                User = new { familiar.IdUsuarioExterno, familiar.Nombre, familiar.Email }
            });
        }
        else
        {
            //tabla Pacientes
            var user = _context.Pacientes
                .Include(p => p.Psiquiatra)
                .FirstOrDefault(u => u.Contrasena == request.Password && u.Email == request.Email);

            if (user == null)
            {
                Console.WriteLine("Paciente no encontrado");
                return Unauthorized(new { Success = false, Message = "Credenciales inválidas" });
            }

            Console.WriteLine($"Paciente encontrado: {user.Nombre}, {user.Email}");
            return Ok(new
            {
                Success = true,
                TipoUsuario = "Paciente",
                User = new
                {
                    user.ID_Paciente,
                    user.Nombre,
                    user.Email,
                    user.ID_Psiquiatra
                }
            });
        }
    }

}