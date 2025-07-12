using System.Collections.Generic;
using System.Threading.Tasks;
using WebConTablas.Models;

public interface IPreguntaService
{
    Task<List<Pregunta>> ObtenerTodasAsync();
    Task<Pregunta?> ObtenerPorIdAsync(int id);
    Task CrearAsync(Pregunta pregunta);
    Task EditarAsync(Pregunta pregunta);
    Task EliminarAsync(int id);
}
