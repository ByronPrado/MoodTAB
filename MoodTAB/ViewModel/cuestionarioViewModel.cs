using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoodTAB.Models;
using System.Threading.Tasks;

namespace MoodTAB.ViewModel
{
    public partial class Cuestionario : ObservableObject
    {
        [ObservableProperty]
        Pregunta preguntaActual = null!;

        [ObservableProperty]
        string respuestaUsuario = string.Empty;

        public Cuestionario()
        {
            _ = ObtenerPreguntaAleatoria();
        }

        private async Task ObtenerPreguntaAleatoria()
        {
            var preguntas = await App.Database.GetQuestionsAsync();
            if (preguntas.Any())
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
        }
    }
}
