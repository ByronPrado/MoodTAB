// Ruta: /Services/FormularioService.cs

using Microsoft.EntityFrameworkCore;
using WebConTablas.Models;

public class FormularioService : IFormularioService
{
    private readonly AppDbContext _context;

    public FormularioService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Formulario>> ObtenerTodosAsync()
    {
        return await _context.Formularios.Include(f => f.Psiquiatra).ToListAsync();
    }

    public async Task<Formulario?> ObtenerDetallesAsync(int id)
    {
        return await _context.Formularios
            .Include(f => f.Psiquiatra)
            .Include(f => f.Preguntas)
                .ThenInclude(fp => fp.Pregunta)
            .FirstOrDefaultAsync(f => f.ID_Formulario == id);
    }

    public async Task CrearAsync(Formulario formulario)
    {
        formulario.Created_at = DateTime.Now;
        _context.Add(formulario);
        await _context.SaveChangesAsync();
    }

    public async Task EditarAsync(Formulario formulario)
    {
        _context.Update(formulario);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(int id)
    {
        var formulario = await _context.Formularios.FindAsync(id);
        if (formulario != null)
        {
            _context.Formularios.Remove(formulario);
            await _context.SaveChangesAsync();
        }
    }
}
