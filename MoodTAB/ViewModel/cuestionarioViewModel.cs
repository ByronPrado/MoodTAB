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

        public bool EsAbierta => Pregunta?.Tipo_Pregunta == "Abierta";
        public bool EsEscala => Pregunta?.Tipo_Pregunta == "Escala";
        public bool EsSeleccion => Pregunta?.Tipo_Pregunta == "Seleccion";
        public int MinimoEscala { get; set; } = 0;
        public int MaximoEscala { get; set; } = 10;
        public List<string> OpcionesSeleccion = [];
    }

    public partial class Cuestionario : ObservableObject
    {

        //private readonly MainViewModel _mainViewModel;

        [ObservableProperty]
        ObservableCollection<PreguntaConRespuesta> preguntasConRespuesta = new();

        [ObservableProperty]
        ObservableCollection<Respuestas> respuestasLista = new();

        [ObservableProperty]
        int respuestaUsuarioEscala;

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
            var preguntas = await App.Database.GetQuestionsByUserIdAsync(Globals.id_usuario);
            var lista = preguntas.Select(p =>
            {
                var preguntaRespuesta = new PreguntaConRespuesta
                {
                    Pregunta = p,
                    RespuestaUsuario = string.Empty
                };

                if (p.Tipo_Pregunta == "Escala" && !string.IsNullOrWhiteSpace(p.Opciones_Seleccion))
                {
                    var partes = p.Opciones_Seleccion.Split(',');

                    if (partes.Length == 2 &&
                        int.TryParse(partes[0], out int min) &&
                        int.TryParse(partes[1], out int max))
                    {
                        preguntaRespuesta.MinimoEscala = min;
                        preguntaRespuesta.MaximoEscala = max;
                    }
                }

                if (p.Tipo_Pregunta == "Seleccion" && !string.IsNullOrWhiteSpace(p.Opciones_Seleccion))
                {
                    preguntaRespuesta.OpcionesSeleccion =  p.Opciones_Seleccion.Split(',').Select(o => o.Trim()).ToList();
                }

                return preguntaRespuesta;
            });

            PreguntasConRespuesta = new ObservableCollection<PreguntaConRespuesta>(lista);
        }




        [RelayCommand]
        private async Task GuardarRespuestas()
        {
            // Verifica si alguna respuesta está vacía
            var noRespondidas = PreguntasConRespuesta
                .Where(item => string.IsNullOrWhiteSpace(item.RespuestaUsuario))
                .ToList();

            if (noRespondidas.Any())
            {
                // Puedes mostrar el texto de la primera pregunta no respondida, por ejemplo
                var pregunta = noRespondidas.First().Pregunta;
                await Shell.Current.DisplayAlert(
                    "Respuesta vacía",
                    $"Por favor, responde todas las preguntas antes de guardar.\nFalta: '{pregunta.Texto_Pregunta}'",
                    "OK");
                return;
            }

            // Si todas están respondidas, guarda normalmente
            foreach (var item in PreguntasConRespuesta)
            {
                var respuesta = new Respuestas
                {
                    Texto_Respuesta = item.RespuestaUsuario,
                    PreguntaId = item.Pregunta.Id,
                    CreatedAt = DateTime.Now
                };

                await App.Database.SaveAnswerAsync(respuesta);
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
        [RelayCommand]
        private void SeleccionarSi(PreguntaConRespuesta pregunta)
        {
            if (pregunta != null)
                pregunta.RespuestaUsuario = "SI";
        }

        [RelayCommand]
        private void SeleccionarNo(PreguntaConRespuesta pregunta)
        {
            if (pregunta != null)
                pregunta.RespuestaUsuario = "NO";
        }
   
    }
    
}
