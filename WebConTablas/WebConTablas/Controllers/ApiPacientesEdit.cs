using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using WebConTablas.Models;

[ApiController]
[Route("api/[controller]")]
public class ApiPacientesEdit : ControllerBase
{
    private readonly AppDbContext _context;

    public ApiPacientesEdit(AppDbContext context)
    {
        _context = context;
    }

    public class PacienteUpdateDto
    {
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdatePaciente(int id, [FromBody] PacienteUpdateDto dto)
    {
        string anterior = "0";
        string actual = "0";
        var paciente = await _context.Pacientes.FindAsync(id);
        if (paciente == null)
        {
            Console.WriteLine($"Paciente con ID {id} no encontrado.");
            return NotFound();
        }
        if (paciente.Nombre != dto.Nombre)
        {
            anterior += $"Nombre:{paciente.Nombre}/";
            paciente.Nombre = dto.Nombre;
            actual += $"Nombre:{paciente.Nombre}/";

        }
        if (paciente.Email != dto.Email)
        {
            anterior += $"EMail:{paciente.Email}/";
            paciente.Email = dto.Email;
            actual += $"EMail:{paciente.Email}/";

        }
        if (paciente.Telefono != dto.Telefono)
        {
            anterior += $"Telefono:{paciente.Telefono}/";
            paciente.Telefono = dto.Telefono;
            actual += $"Telefono:{paciente.Telefono}/";
        }
//log en bd
        var log = new Logs
        {
            ID_Paciente = paciente.ID_Paciente,
            ID_Psiquiatra = paciente.ID_Psiquiatra ?? 0,
            TipoLog = "UpdatePaciente",
            Anterior = anterior,
            Actual = actual,
            Fecha = DateTime.UtcNow
        };
        _context.Logs.Add(log);
        await _context.SaveChangesAsync();
        return Ok();
    }
}