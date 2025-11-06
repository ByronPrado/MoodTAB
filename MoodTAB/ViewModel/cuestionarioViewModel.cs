using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoodTAB.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Text.Json;
using MoodTAB.Services;
using MoodTAB.Vistas;
using Microsoft.Maui.Controls;

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
            foreach (var item in OpcionesSeleccion)
                item.IsSelected = item.Texto == opcion;
        }
    }

    public class CuestionarioData
    {
        public int IdAsignacion { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public ObservableCollection<PreguntaConRespuesta> PreguntasConRespuesta { get; set; } = new();
    }
    public partial class Cuestionario : ObservableObject
    {
        [ObservableProperty] bool pendiente;
        [ObservableProperty] bool nopendiente;
        [ObservableProperty] string prueba;
        [ObservableProperty] string log_test = string.Empty;

        [ObservableProperty] ObservableCollection<CuestionarioData> listaCuestionarios = new();

        [ObservableProperty] private CuestionarioData cuestionarioSeleccionado;
        private int idAsignacion;

        public async Task InitializeAsync()
        {
            try
            {
                await SepararCuestionarios();
            }
            catch (Exception ex)
            {
                Log_test = "fallosepararcuestionarios: " + ex.Message;

            }
        }

        public Cuestionario()
        {
            pendiente = Globals.cuestionario_pendiente;
            nopendiente = !pendiente;
            //Task.Run(async () => await SepararCuestionarios());
        }

        public void SetIdAsignacion(int id)
        {
            idAsignacion = id;
        }
        public async Task SepararCuestionarios()
        {
            ListaCuestionarios.Clear();
            var url = $"{Globals.direccion_ngrok}api/formulario/{Globals.id_paciente_DB}";
            using var client = new HttpClient()
            {
                Timeout = TimeSpan.FromSeconds(10)
            };
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                Pendiente = false;
                Nopendiente = true;
                return;
            }

            var content = await response.Content.ReadAsStringAsync();
            Globals.cuestionario = content;
            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;
           //Log_test = len.ToString();
            if (root.ValueKind != JsonValueKind.Array)
            {
                Log_test = "El JSON recibido no es una lista de cuestionarios.";
                return;
            }

            var len = doc.RootElement.GetArrayLength();
            if (len == 0)
            {
                Log_test = $" largo = {len}\t";
                Pendiente = false;
                Nopendiente = true;

            }
            else
            {
                Pendiente = true;
                Nopendiente = false;
            }
            foreach (var cuestionarioJson in root.EnumerateArray())
            {
                // Leer ID asignación
                int idAsignacion = cuestionarioJson.GetProperty("iD_Asignacion").GetInt32();

                // Extraer formulario
                if (cuestionarioJson.TryGetProperty("formulario", out var formulario) &&
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

                    // Aquí ya tienes un cuestionario completo 
                    var cuestionario = new CuestionarioData();
                    cuestionario.IdAsignacion = idAsignacion;
                    cuestionario.Titulo = $"Cuestionario {idAsignacion}";
                    //cuestionario.Grupo = "null";
                    cuestionario.PreguntasConRespuesta = new ObservableCollection<PreguntaConRespuesta>(lista);

                    //una colección en memoria de los cuestionarios
                    ListaCuestionarios.Add(cuestionario);
                }
            }

            //Log_test += $"Se procesaron {root.GetArrayLength()} cuestionarios.";
        }

        [RelayCommand]
        public async Task SeleccionarCuestionario(CuestionarioData value)
        {
            if (value == null) return;

            var detalleVm = new CuestionarioDetalleViewmodel();
            detalleVm.SetPreguntas(value.PreguntasConRespuesta ?? new(), value.IdAsignacion);

            // Guarda el ViewModel en el contenedor estático
            NavigationState.DetalleVm = detalleVm;

            // Ahora la ruta funciona
            await Shell.Current.GoToAsync(nameof(CuestionarioDetallePage));
        }

        public static class NavigationState
        {
            public static CuestionarioDetalleViewmodel DetalleVm { get; set; }
        }
        partial void OnCuestionarioSeleccionadoChanged(CuestionarioData value)
        {
            if (value == null)
            {
                Log_test = "value = null";
                return;
            }

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (Shell.Current == null)
                {
                    Log_test = "Shell.Current = null";
                    return;
                }

                if (Shell.Current.Navigation == null)
                {
                    Log_test = "Shell.Current.Navigation = null";
                    return;
                }

                if (value.PreguntasConRespuesta == null)
                {
                    Log_test = "PreguntasConRespuesta = null";
                    return;
                }

                try
                {
                    var detalleVm = new CuestionarioDetalleViewmodel();
                    detalleVm.SetPreguntas(value.PreguntasConRespuesta, value.IdAsignacion);

                    var detallePage = new CuestionarioDetallePage
                    {
                        BindingContext = detalleVm
                    };

                    await Shell.Current.Navigation.PushAsync(detallePage);
                }
                catch (Exception ex)
                {
                    Log_test = "catch: "+ex.ToString();
                }
            });
        }

    }    
}
