using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        var paciente = await _context.Pacientes.FindAsync(id);
        if (paciente == null)
        {
            Console.WriteLine($"Paciente con ID {id} no encontrado.");
            return NotFound();
        }

        paciente.Nombre = dto.Nombre;
        paciente.Email = dto.Email;
        paciente.Telefono = dto.Telefono;

        await _context.SaveChangesAsync();
        return Ok(paciente);
    }
}