using System.Collections.Generic;
using System.Threading.Tasks;
using WebConTablas.Models; // ✅ Asegúrate de incluir esto

public interface IVistaDatosService
{
    Task<List<Psiquiatra>> ObtenerPsiquiatrasAsync();
    Task<List<Paciente>> ObtenerPacientesAsync();
    Task<List<Formulario>> ObtenerFormulariosAsync();
}
