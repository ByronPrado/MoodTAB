using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebConTablas.Models;

namespace WebConTablas.Services
{
    public class PacienteService : IPacienteService
    {
        private readonly AppDbContext _context;

        public PacienteService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Paciente>> ObtenerPacientesPorPsiquiatraAsync(int idPsiquiatra)
        {
            return await _context.Pacientes
                .Where(p => p.ID_Psiquiatra == idPsiquiatra)
                .Include(p => p.DiariosEmocionales)
                .Include(p => p.FormulariosAsignados)
                    .ThenInclude(fa => fa.Formulario)
                .ToListAsync();
        }

        public async Task<Paciente?> ObtenerPacientePorIdAsync(int id, int idPsiquiatra)
        {
            return await _context.Pacientes
                .FirstOrDefaultAsync(p => p.ID_Paciente == id && p.ID_Psiquiatra == idPsiquiatra);
        }

        public async Task CrearPacienteAsync(Paciente paciente)
        {
            _context.Pacientes.Add(paciente);
            await _context.SaveChangesAsync();
        }

        public async Task EditarPacienteAsync(Paciente paciente)
        {
            _context.Pacientes.Update(paciente);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarPacienteAsync(int id, int idPsiquiatra)
        {
            var paciente = await ObtenerPacientePorIdAsync(id, idPsiquiatra);
            if (paciente != null)
            {
                _context.Pacientes.Remove(paciente);
                await _context.SaveChangesAsync();
            }
        }
    }
}
