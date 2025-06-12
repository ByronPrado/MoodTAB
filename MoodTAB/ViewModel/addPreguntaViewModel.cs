using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoodTAB.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MoodTAB.ViewModel
{
    public partial class AddPregunta : ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<Pregunta> preguntas = new();

        [ObservableProperty]
        string nuevaPregunta;

        public AddPregunta()
        {
            NuevaPregunta = string.Empty;
            _ = LoadPreguntasAsync();
        }

        private async Task LoadPreguntasAsync()
        {
            var items = await App.Database.GetQuestionsAsync();
            Preguntas = new ObservableCollection<Pregunta>(items);
        }

        [RelayCommand]
        private async Task AgregarPregunta()
        {
            if (string.IsNullOrWhiteSpace(NuevaPregunta))
                return;

            var pregunta = new Pregunta
            {
                Texto_Pregunta = NuevaPregunta,
                CreatedAt = DateTime.Now
            };

            await App.Database.SaveQuestionAsync(pregunta);
            NuevaPregunta = string.Empty;
            await LoadPreguntasAsync();
        }
    [RelayCommand]
        private async Task EliminarPregunta(Pregunta pregunta)
        {
            if (pregunta is null)
            {
                await Shell.Current.DisplayAlert("pregunta nula", "No se puede eliminar una pregunta nula.", "OK"); 
                return;
            }
            bool confirm = await Shell.Current.DisplayAlert("Confirmar", "¿Eliminar esta pregunta?", "Sí", "No");

            if (confirm)
            {
                await App.Database.DeleteQuestionAsync(pregunta);
                Preguntas.Remove(pregunta); // Actualiza solo la colección, más eficiente
            }
        }
    }
}
