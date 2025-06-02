using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoodTAB.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MoodTAB.ViewModel
{
    public partial class Cuestionario : ObservableObject
    {
        [ObservableProperty]
        Pregunta preguntaActual = null!;

        [ObservableProperty]
        ObservableCollection<Respuestas> respuestasLista = new();
        [ObservableProperty]
        string respuestaUsuario = string.Empty;

        public Cuestionario()
        {
            Task.Run(async () =>
            {
                await ObtenerPreguntaAleatoria();
                await CargarRespuestas();
            });
        }

        private async Task ObtenerPreguntaAleatoria()
        {
            var preguntas = await App.Database.GetQuestionsAsync();
            if (preguntas.Count != 0)
            {
                var random = new Random();
                PreguntaActual = preguntas[random.Next(preguntas.Count)];

            }
        }

        [RelayCommand]
        private async Task GuardarRespuesta()
        {
            if (PreguntaActual == null || string.IsNullOrWhiteSpace(RespuestaUsuario))
                return;

            var respuesta = new Respuestas
            {
                Texto_Respuesta = RespuestaUsuario,
                PreguntaId = PreguntaActual.Id,
                CreatedAt = DateTime.Now
            };

            await App.Database.SaveAnswerAsync(respuesta);
            RespuestaUsuario = string.Empty;

            await ObtenerPreguntaAleatoria(); // para responder otra
            await CargarRespuestas(); // para cargar las respuestas

        }

        private async Task CargarRespuestas()
        {
            var todas = await App.Database.GetAnswersAsync();

            foreach (var respuesta in todas)
            {
                respuesta.Pregunta = await App.Database.GetQuestionByIdAsync(respuesta.PreguntaId);
            }

            RespuestasLista = new ObservableCollection<Respuestas>(todas);
        }
        [RelayCommand]
        private async Task BorrarBaseDatos()
        {
            // 1) Obtener todos los ítems marcados como IsDone == true
            var doneItems = RespuestasLista.ToList();

            // 2) Para cada uno, borrarlo de la base de datos
            foreach (Respuestas res in doneItems)
            {
                await App.Database.DeleteAnswersAsync(res);
            }

            // 3) Recargar la lista desde la BD (se vaciará el contenido eliminado)
            await CargarRespuestas();
        }
    }
}
