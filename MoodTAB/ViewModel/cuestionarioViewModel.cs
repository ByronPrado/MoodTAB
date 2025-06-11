using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoodTAB.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MoodTAB.ViewModel
{

    public partial class PreguntaConRespuesta : ObservableObject
    {
        public Pregunta Pregunta { get; set; } = null!;

        [ObservableProperty]
        private string respuestaUsuario = string.Empty;
    }

    public partial class Cuestionario : ObservableObject
    {

        //private readonly MainViewModel _mainViewModel;

        [ObservableProperty]
        ObservableCollection<PreguntaConRespuesta> preguntasConRespuesta = new();

        [ObservableProperty]
        ObservableCollection<Respuestas> respuestasLista = new();

        public Cuestionario()
        {
            //_mainViewModel = mainViewModel;

            Task.Run(async () =>
            {
                await CargarPreguntas();
                await CargarRespuestas();
            });
        }

        private async Task CargarPreguntas()
        {
            var preguntas = await App.Database.GetQuestionsAsync();
            var lista = preguntas.Select(p => new PreguntaConRespuesta
            {
                Pregunta = p,
                RespuestaUsuario = string.Empty
            });

            PreguntasConRespuesta = new ObservableCollection<PreguntaConRespuesta>(lista);
        }



        [RelayCommand]
        private async Task GuardarRespuestas()
        {
            foreach (var item in PreguntasConRespuesta)
            {
                if (!string.IsNullOrWhiteSpace(item.RespuestaUsuario))
                {
                    var respuesta = new Respuestas
                    {
                        Texto_Respuesta = item.RespuestaUsuario,
                        PreguntaId = item.Pregunta.Id,
                        CreatedAt = DateTime.Now
                    };

                    await App.Database.SaveAnswerAsync(respuesta);
                }
            }

            await CargarRespuestas();
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
            var doneItems = RespuestasLista.ToList();
            foreach (Respuestas res in doneItems)
            {
                await App.Database.DeleteAnswersAsync(res);
            }
            await CargarRespuestas();
        }
    
    }
}
