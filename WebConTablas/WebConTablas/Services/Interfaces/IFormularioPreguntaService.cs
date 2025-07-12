using WebConTablas.Models;

public interface IFormularioPreguntaService
{
    Formulario ObtenerFormularioConPreguntas(int id);
    List<Pregunta> ObtenerPreguntasNoAsignadas(int formularioId);
    Task AsignarPreguntasAsync(int formularioId, int[] preguntasIds);
    Task DesasignarPreguntaAsync(int formularioId, int preguntaId);
}
