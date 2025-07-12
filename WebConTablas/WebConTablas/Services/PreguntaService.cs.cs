using System.Collections.Generic;
using System.Threading.Tasks;
using WebConTablas.Models;
using Microsoft.EntityFrameworkCore;

public class PreguntaService : IPreguntaService
{
    private readonly AppDbContext _context;
    public PreguntaService(AppDbContext context) => _context = context;

    public async Task<List<Pregunta>> ObtenerTodasAsync()
        => await _context.Preguntas.ToListAsync();

    public async Task<Pregunta?> ObtenerPorIdAsync(int id)
        => await _context.Preguntas.FindAsync(id);

    public async Task CrearAsync(Pregunta pregunta)
    {
        _context.Preguntas.Add(pregunta);
        await _context.SaveChangesAsync();
    }

    public async Task EditarAsync(Pregunta pregunta)
    {
        _context.Preguntas.Update(pregunta);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(int id)
    {
        var pregunta = await ObtenerPorIdAsync(id);
        if (pregunta != null)
        {
            _context.Preguntas.Remove(pregunta);
            await _context.SaveChangesAsync();
        }
    }
}
