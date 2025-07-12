
using WebConTablas.Models;

public interface IFormularioService
{
    Task<List<Formulario>> ObtenerTodosAsync();
    Task<Formulario?> ObtenerDetallesAsync(int id);
    Task CrearAsync(Formulario f);
    Task EditarAsync(Formulario f);
    Task EliminarAsync(int id);
}
