using WebConTablas.Models;
using Microsoft.EntityFrameworkCore;

public class FormularioAsignadoService : IFormularioAsignadoService
{
    private readonly AppDbContext _context;

    public FormularioAsignadoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<FormularioAsignado>> ObtenerTodosAsync()
    {
        return await _context.FormulariosAsignados
            .Include(fa => fa.Formulario)
            .Include(fa => fa.Paciente)
            .ToListAsync();
    }

    public async Task AsignarFormularioAsync(int ID_Formulario, int ID_Paciente, DateTime? Fecha_Limite)
    {
        if (Fecha_Limite != null)
        {
            DateTime Fecha_ven = Fecha_Limite.Value.Date;
            Fecha_ven = DateTime.SpecifyKind(Fecha_ven, DateTimeKind.Local).ToUniversalTime(); // Forzar Local y convertir a UTC
            Fecha_Limite = Fecha_ven;

        }
        else Fecha_Limite = DateTime.UtcNow.Date + TimeSpan.FromDays(7);
        var asignacion = new FormularioAsignado
        {
            ID_Formulario = ID_Formulario,
            ID_Paciente = ID_Paciente,
            Fecha_Asignacion = DateTime.UtcNow,
            Fecha_Limite = Fecha_Limite,
            Estado = "pendiente"
        };

        _context.FormulariosAsignados.Add(asignacion);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(int id)
    {
        var asignacion = await _context.FormulariosAsignados.FindAsync(id);
        if (asignacion != null)
        {
            _context.FormulariosAsignados.Remove(asignacion);
            await _context.SaveChangesAsync();
        }
    }
}
