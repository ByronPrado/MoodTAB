using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoodTAB.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Text.Json;

namespace MoodTAB.ViewModel
{

    public partial class PreguntaConRespuesta : ObservableObject
    {
        private Pregunta _pregunta;
        public Pregunta Pregunta
        {
            get => _pregunta;
            set
            {
                SetProperty(ref _pregunta, value);
                OnPropertyChanged(nameof(EsAbierta));
                OnPropertyChanged(nameof(EsEscala));
                OnPropertyChanged(nameof(EsSeleccion));
            }
        }
        

        [ObservableProperty]
        private string respuestaUsuario = string.Empty;

        public bool EsAbierta => Pregunta?.Tipo == "Abierta";
        public bool EsEscala => Pregunta?.Tipo == "Escala";
        public bool EsSeleccion => Pregunta?.Tipo == "Seleccion";
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
            var url = $"http://10.0.2.2:5051/api/formulario/{Globals.id_usuario}";
            using var client = new HttpClient();
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return;

            var json = await response.Content.ReadAsStringAsync();

            // Usa System.Text.Json para navegar el JSON y extraer las preguntas
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            // Navega hasta formulario.preguntas
            if (root.TryGetProperty("formulario", out var formulario) &&
                formulario.TryGetProperty("preguntas", out var preguntasJson) &&
                preguntasJson.ValueKind == JsonValueKind.Array)
            {
                var lista = new List<PreguntaConRespuesta>();

                foreach (var preguntaJson in preguntasJson.EnumerateArray())
                {
                    var pregunta = new Pregunta
                    {
                        ID_Pregunta = preguntaJson.GetProperty("iD_Pregunta").GetInt32(),
                        Contenido = preguntaJson.GetProperty("contenido").GetString(),
                        Tipo = preguntaJson.GetProperty("tipo").GetString(),
                        Extra = preguntaJson.TryGetProperty("extra", out var extraProp) ? extraProp.GetString() : null
                        // Agrega más campos si los necesitas
                    };

                    var preguntaRespuesta = new PreguntaConRespuesta
                    {
                        Pregunta = pregunta,
                        RespuestaUsuario = string.Empty
                    };


                    if (pregunta.Tipo == "Escala")
                    {
                        if (preguntaJson.TryGetProperty("escalaMin", out var minProp) && minProp.ValueKind == JsonValueKind.Number)
                            preguntaRespuesta.MinimoEscala = minProp.GetInt32();
                        if (preguntaJson.TryGetProperty("escalaMax", out var maxProp) && maxProp.ValueKind == JsonValueKind.Number)
                            preguntaRespuesta.MaximoEscala = maxProp.GetInt32();
                    }
                    if (pregunta.Tipo == "Seleccion")
                    {
                        if (preguntaJson.TryGetProperty("opcionesSeleccion", out var opcionesProp) && opcionesProp.ValueKind == JsonValueKind.String)
                            preguntaRespuesta.OpcionesSeleccion = opcionesProp.GetString()?.Split(',').Select(o => o.Trim()).ToList() ?? new List<string>();
                    }

                    lista.Add(preguntaRespuesta);
                }

                PreguntasConRespuesta = new ObservableCollection<PreguntaConRespuesta>(lista);
            }
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
                    $"Por favor, responde todas las preguntas antes de guardar.\nFalta: '{pregunta.Contenido}'",
                    "OK");
                return;
            }

            // Si todas están respondidas, guarda normalmente
            foreach (var item in PreguntasConRespuesta)
            {
                var respuesta = new Respuestas
                {
                    Texto_Respuesta = item.RespuestaUsuario,
                    PreguntaId = item.Pregunta.ID_Pregunta,
                    CreatedAt = DateTime.Now
                };

                await App.Database.SaveAnswerAsync(respuesta);
            }
            var payload = new
            {
                ID_Asignacion = 9,
                Respuestas = PreguntasConRespuesta.Select(p => new {
                    ID_Pregunta = p.Pregunta.ID_Pregunta,
                    Contenido = p.RespuestaUsuario
                }).ToList()
            };

            var url = "http://10.0.2.2:5051/api/formulario/responder";
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            using var client = new HttpClient();
            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                await Shell.Current.DisplayAlert("¡Listo!", "Respuestas enviadas correctamente.", "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "No se pudieron enviar las respuestas.", "OK");
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
