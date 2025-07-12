using WebConTablas.Models;

public interface IFormularioAsignadoService
{
    Task<List<FormularioAsignado>> ObtenerTodosAsync();
    Task AsignarFormularioAsync(int ID_Formulario, int ID_Paciente, DateTime? Fecha_Limite);
    Task EliminarAsync(int id);
}
