using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebConTablas.Models; // ✅ Necesario
public class VistaDatosService : IVistaDatosService
{
    private readonly AppDbContext _context;

    public VistaDatosService(AppDbContext context)
    {
        _context = context;
    }

    public Task<List<Psiquiatra>> ObtenerPsiquiatrasAsync() => _context.Psiquiatras.ToListAsync();
    public Task<List<Paciente>> ObtenerPacientesAsync() => _context.Pacientes.ToListAsync();
    public Task<List<Formulario>> ObtenerFormulariosAsync() => _context.Formularios.ToListAsync();
}
