using System.Collections.Generic;
using System.Threading.Tasks;
using WebConTablas.Models;

namespace WebConTablas.Services  // <-- Agrega este namespace para organizar
{
    public interface IPacienteService
    {
        Task<List<Paciente>> ObtenerPacientesPorPsiquiatraAsync(int idPsiquiatra);
        Task<Paciente?> ObtenerPacientePorIdAsync(int id, int idPsiquiatra);
        Task CrearPacienteAsync(Paciente paciente);
        Task EditarPacienteAsync(Paciente paciente);
        Task EliminarPacienteAsync(int id, int idPsiquiatra);
    }
}
