using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoodTAB.Models;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace MoodTAB.ViewModel
{
    
    public partial class CuestionarioDetalleViewmodel : ObservableObject
    {
        [ObservableProperty]
        string log_test = string.Empty;
        [ObservableProperty]
        int respuestaUsuarioEscala;

        [ObservableProperty]
        ObservableCollection<PreguntaConRespuesta> preguntasConRespuesta = new();

        [ObservableProperty]
        ObservableCollection<Respuestas> respuestasLista = new();
        private int idAsignacion;
        public void SetPreguntas(ObservableCollection<PreguntaConRespuesta> preguntas, int idAsignacion)
        {
            PreguntasConRespuesta = preguntas;
            this.idAsignacion = idAsignacion;
        }

        public void SetIdAsignacion(int id)
        {
            idAsignacion = id;
        } 
        [RelayCommand]
        private async Task GuardarRespuestas()
        {
            try
            {
                if (PreguntasConRespuesta == null || !PreguntasConRespuesta.Any())
                {
                    Application.Current.MainPage.DisplayAlert("Error", "No hay preguntas cargadas.", "OK");
                    return;
                }

                // Filtra items nulos o preguntas nulas o respuestas vacías
                var preguntasInvalidas = PreguntasConRespuesta
                    .Where(x => x == null || x.Pregunta == null || string.IsNullOrWhiteSpace(x.RespuestaUsuario))
                    .ToList();

                if (preguntasInvalidas.Any())
                {
                    var primeraPregunta = preguntasInvalidas.FirstOrDefault()?.Pregunta?.Contenido ?? "Pregunta desconocida";
                    
                    await Application.Current.MainPage.DisplayAlert("Respuesta vacía",
                        $"Por favor, responde todas las preguntas antes de guardar.\nFalta: '{primeraPregunta}'",
                        "OK");
                    return;
                }

                // Prepara payload solo con items válidos
                var respuestasPayload = PreguntasConRespuesta
                    .Where(p => p != null && p.Pregunta != null)
                    .Select(p => new
                    {
                        ID_Pregunta = p.Pregunta.ID_Pregunta,
                        Contenido = p.RespuestaUsuario ?? string.Empty
                    })
                    .ToList();

                var payload = new
                {
                    ID_Asignacion = idAsignacion,
                    Respuestas = respuestasPayload
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                using var client = new HttpClient();
                var response = await client.PostAsync("http://10.0.2.2:5051/api/formulario/responder", content);

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    if (response != null && response.IsSuccessStatusCode)
                    {
                        await Application.Current.MainPage.DisplayAlert("¡Listo!", "Respuestas enviadas correctamente.", "OK");
                        await Application.Current.MainPage.Navigation.PopAsync();

                    }
                    else
                    {
                        var errorMsg = response != null ? await response.Content.ReadAsStringAsync() : "No hubo respuesta del servidor";
                        await Application.Current.MainPage.DisplayAlert("Error", $"No se pudieron enviar las respuestas.\n{errorMsg}", "OK");
                    }
                });
            }
            catch (Exception ex)
            {
                Application.Current.MainPage.DisplayAlert("Excepción", ex.ToString(), "OK");
            }
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