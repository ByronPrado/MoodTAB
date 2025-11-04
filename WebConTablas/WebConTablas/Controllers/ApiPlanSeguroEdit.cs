using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebConTablas.Models;

[ApiController]
[Route("api/[controller]")]
public class ApiPlanSeguroEdit : ControllerBase
{
    private readonly AppDbContext _context;

    public ApiPlanSeguroEdit(AppDbContext context)
    {
        _context = context;
    }

    public class PlanUpdateDto
    {
        public string PlanJson { get; set; } = "";
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdatePlanSeguro(int id, [FromBody] PlanUpdateDto dto)
    {
        var paciente = await _context.Pacientes.FindAsync(id);
        if (paciente == null)
        {
            Console.WriteLine($"Paciente con ID {id} no encontrado.");
            return NotFound();
        }

        var anterior = paciente.PlanSeguro ?? "";
        paciente.PlanSeguro = dto.PlanJson;
        await _context.SaveChangesAsync();

        // Registramos el log
        var log = new Logs
        {
            ID_Paciente = paciente.ID_Paciente,
            ID_Psiquiatra = paciente.ID_Psiquiatra??0,
            TipoLog = "UpdatePlanSeguro",
            Anterior = anterior,
            Actual = dto.PlanJson,
            Fecha = DateTime.UtcNow
        };

        _context.Logs.Add(log);
        await _context.SaveChangesAsync();
        return Ok();
    }
}