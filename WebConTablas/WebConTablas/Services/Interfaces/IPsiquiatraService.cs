using System.Collections.Generic;
using System.Threading.Tasks;
using WebConTablas.Models;

public interface IPsiquiatraService
{
    Task<List<Psiquiatra>> ObtenerTodosAsync();
    Task<Psiquiatra?> BuscarPorCredencialesAsync(string nombre, string email);
    Task<Psiquiatra?> ObtenerPorIdAsync(int id);
    Task CrearAsync(Psiquiatra psiquiatra);
    Task EditarAsync(Psiquiatra psiquiatra);
    Task EliminarAsync(int id);
}
