using Microsoft.AspNetCore.Mvc;
using WebConTablas.Models;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/diarios")]
public class ApiDiariosController : ControllerBase
{
    private readonly AppDbContext _context;
    public ApiDiariosController(AppDbContext context) => _context = context;

    [HttpGet("{pacienteId}")]
    public IActionResult GetDiariosPaciente(int pacienteId)
    {
        // Calcular la fecha de hace un mes
        var fechaHaceUnMes = DateTime.UtcNow.AddMonths(-1);
        
        var diarios = _context.DiariosEmocionales
            .Where(d => d.ID_Paciente == pacienteId && d.Fecha >= fechaHaceUnMes)
            .OrderByDescending(d => d.Fecha)
            .Select(d => new 
            {
                d.ID_Diario,
                d.ID_Paciente,
                d.Fecha,
                d.Descripcion,
                d.Estado,
                // Biometría
                d.HR_RitmoCardiaco,
                d.HRV_VariabilidadFrecuencia,
                d.Pasos,
                d.Hora_dormida,
                d.Horas_celular,
                d.Horas_redes,
                // Emociones
                d.Emociones,
                // Propiedades calculadas
                d.Coherencia,
                d.Errores_gramaticales,
                d.Ultima_Revision_Psiquiatra
            })
            .ToList(); 
         
        if (diarios == null || !diarios.Any())
            return NotFound(new { mensaje = "No se encontraron diarios emocionales para este paciente en el último mes" });

        return Ok(new 
        { 
            total = diarios.Count,
            periodo = $"Últimos 30 días (desde {fechaHaceUnMes:yyyy-MM-dd})",
            diarios
        });
    }
}