using WebConTablas.Models;
using Microsoft.EntityFrameworkCore;

public class FormularioPreguntaService : IFormularioPreguntaService
{
    private readonly AppDbContext _context;

    public FormularioPreguntaService(AppDbContext context)
    {
        _context = context;
    }

    public Formulario ObtenerFormularioConPreguntas(int id)
    {
        return _context.Formularios
            .Include(f => f.Preguntas)
            .FirstOrDefault(f => f.ID_Formulario == id)!;
    }

    public List<Pregunta> ObtenerPreguntasNoAsignadas(int formularioId)
    {
        var idsAsignadas = _context.FormularioPreguntas
            .Where(fp => fp.ID_Formulario == formularioId)
            .Select(fp => fp.ID_Pregunta)
            .ToList();

        return _context.Preguntas
            .Where(p => !idsAsignadas.Contains(p.ID_Pregunta))
            .ToList();
    }

    public async Task AsignarPreguntasAsync(int formularioId, int[] preguntasIds)
    {
        foreach (var idPregunta in preguntasIds)
        {
            if (!_context.FormularioPreguntas.Any(fp => fp.ID_Formulario == formularioId && fp.ID_Pregunta == idPregunta))
            {
                _context.FormularioPreguntas.Add(new FormularioPregunta
                {
                    ID_Formulario = formularioId,
                    ID_Pregunta = idPregunta,
                    Orden = 1 // puedes ajustar esto
                });
            }
        }
        await _context.SaveChangesAsync();
    }

    public async Task DesasignarPreguntaAsync(int formularioId, int preguntaId)
    {
        var relacion = await _context.FormularioPreguntas
            .FirstOrDefaultAsync(fp => fp.ID_Formulario == formularioId && fp.ID_Pregunta == preguntaId);

        if (relacion != null)
        {
            _context.FormularioPreguntas.Remove(relacion);
            await _context.SaveChangesAsync();
        }
    }
}
