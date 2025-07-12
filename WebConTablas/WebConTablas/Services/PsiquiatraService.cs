using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebConTablas.Models;

public class PsiquiatraService : IPsiquiatraService
{
    private readonly AppDbContext _context;

    public PsiquiatraService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Psiquiatra>> ObtenerTodosAsync()
    {
        return await _context.Psiquiatras.ToListAsync();
    }

    public async Task<Psiquiatra?> BuscarPorCredencialesAsync(string nombre, string email)
    {
        return await _context.Psiquiatras.FirstOrDefaultAsync(p => p.Nombre == nombre && p.Email == email);
    }

    public async Task<Psiquiatra?> ObtenerPorIdAsync(int id)
    {
        return await _context.Psiquiatras.FindAsync(id);
    }

    public async Task CrearAsync(Psiquiatra psiquiatra)
    {
        _context.Psiquiatras.Add(psiquiatra);
        await _context.SaveChangesAsync();
    }

    public async Task EditarAsync(Psiquiatra psiquiatra)
    {
        _context.Psiquiatras.Update(psiquiatra);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(int id)
    {
        var psiquiatra = await ObtenerPorIdAsync(id);
        if (psiquiatra != null)
        {
            _context.Psiquiatras.Remove(psiquiatra);
            await _context.SaveChangesAsync();
        }
    }
}
