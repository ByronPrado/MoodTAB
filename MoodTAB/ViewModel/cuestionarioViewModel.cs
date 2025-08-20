using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoodTAB.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Text.Json;

namespace MoodTAB.ViewModel
{
    public class OpcionSeleccionItem : ObservableObject
    {
        public string? Texto { get; set; }
        
        private bool isSelected;
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                if (SetProperty(ref isSelected, value))
                    OnPropertyChanged(nameof(Color));
            }
        }
        public string Color => IsSelected ? "#FF9100" : "#512BD4";
    }
    public partial class PreguntaConRespuesta : ObservableObject
    {
        private Pregunta? _pregunta;
        public Pregunta? Pregunta
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
        [ObservableProperty]
        private string opcionSeleccionada = string.Empty;
        private int _respuestaUsuarioEscala;
        public int RespuestaUsuarioEscala
        {
            get => _respuestaUsuarioEscala;
            set
            {
                if (_respuestaUsuarioEscala != value)
                {
                    _respuestaUsuarioEscala = value;
                    OnPropertyChanged();
                    // Aqui actualizamos las respuesta que guardamos en bd
                    if (EsEscala)
                        RespuestaUsuario = value.ToString();
                }
            }
        }
        public bool EsAbierta => Pregunta?.Tipo == "Abierta";
        public bool EsEscala => Pregunta?.Tipo == "Escala";
        public bool EsSeleccion => Pregunta?.Tipo == "Seleccion";
        public int MinimoEscala { get; set; } = 0;
        public int MaximoEscala { get; set; } = 10;
        public ObservableCollection<OpcionSeleccionItem> OpcionesSeleccion { get; set; } = new();

        [RelayCommand]
        public void SeleccionarOpcion(string opcion)
        {
            OpcionSeleccionada = opcion;
            RespuestaUsuario = opcion;
            // Actualiza el estado de cada opción
            foreach (var item in OpcionesSeleccion)
                item.IsSelected = item.Texto == opcion;
        }
    }

    public partial class Cuestionario : ObservableObject
    {

        [ObservableProperty]
        ObservableCollection<PreguntaConRespuesta> preguntasConRespuesta = new();

        [ObservableProperty]
        ObservableCollection<Respuestas> respuestasLista = new();

        [ObservableProperty]
        int respuestaUsuarioEscala;

        private int idAsignacion;

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
            var url = $"http://10.0.2.2:5051/api/formulario/{Globals.id_paciente_DB}";
            using var client = new HttpClient();
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return;

            var json = await response.Content.ReadAsStringAsync();

            // Usa System.Text.Json para navegar el JSON y extraer las preguntas
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.TryGetProperty("iD_Asignacion", out var asignacionProp))
            {
                idAsignacion = asignacionProp.GetInt32();
            }

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
                        {
                            var opciones = opcionesProp.GetString()?.Split(',').Select(o => o.Trim()).ToList() ?? new List<string>();
                            preguntaRespuesta.OpcionesSeleccion = new ObservableCollection<OpcionSeleccionItem>(
                                opciones.Select(o => new OpcionSeleccionItem { Texto = o })
                            );
                        }
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
                    $"Por favor, responde todas las preguntas antes de guardar.\nFalta: '{pregunta?.Contenido ?? "Pregunta desconocida"}'",
                    "OK");
                return;
            }

            // Si todas están respondidas, guarda normalmente
            foreach (var item in PreguntasConRespuesta)
            {
                var respuesta = new Respuestas
                {
                    Texto_Respuesta = item.RespuestaUsuario,
                    PreguntaId = item.Pregunta != null ? item.Pregunta.ID_Pregunta : 0,
                    CreatedAt = DateTime.Now
                };

                await App.Database.SaveAnswerAsync(respuesta);
            }
            var payload = new
            {
                ID_Asignacion = idAsignacion,
                Respuestas = PreguntasConRespuesta.Select(p => new
                {
                    ID_Pregunta = p.Pregunta != null ? p.Pregunta.ID_Pregunta : 0,
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
                var errorMsg = await response.Content.ReadAsStringAsync();
                await Shell.Current.DisplayAlert("Error", $"No se pudieron enviar las respuestas.\n{errorMsg}", "OK");
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
