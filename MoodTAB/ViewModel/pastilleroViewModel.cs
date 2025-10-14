using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoodTAB.Models;
using Plugin.Maui.Calendar.Models;
using System.Collections.ObjectModel;
using System.Globalization;

namespace MoodTAB.ViewModel
{
    public partial class PastilleroViewModel : ObservableObject
    {
        [ObservableProperty] EventCollection events = new();
        [ObservableProperty] CultureInfo cultura = new("es-ES");
        [ObservableProperty] DateTime shownDate = DateTime.Today;
        [ObservableProperty] string nombreMed = string.Empty;
        [ObservableProperty] string dosisMed = string.Empty;
        [ObservableProperty] ObservableCollection<Medicamento> medicamentosDiaSeleccionado = new();

        public IRelayCommand<DateTime> DiaTocadoCommand { get; }
        public IRelayCommand RegistrarMedicamentoCommand { get; }

        private readonly int usuarioId = int.TryParse(Globals.id_paciente_DB, out int id) ? id : 0;

        public PastilleroViewModel()
        {
            CargarEventos();
            DiaTocadoCommand = new RelayCommand<DateTime>(async fecha => await OnDiaTocado(fecha));
            RegistrarMedicamentoCommand = new RelayCommand(async () => await RegistrarMedicamento());
        }

        private async Task RegistrarMedicamento()
        {
            if (string.IsNullOrWhiteSpace(NombreMed) || string.IsNullOrWhiteSpace(DosisMed))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Por favor completa todos los campos.", "OK");
                return;
            }

            var med = new Medicamento
            {
                Nombre = NombreMed,
                Dosis = DosisMed,
                Usuario_dirigido = usuarioId,
                CreatedAt = DateTime.Now
            };

            await App.Database.SaveMedAsync(med);
            await Application.Current.MainPage.DisplayAlert("Guardado", "Medicamento registrado correctamente.", "OK");

            // Limpia campos
            NombreMed = string.Empty;
            DosisMed = string.Empty;

            // Refresca eventos
            CargarEventos();
            await OnDiaTocado(DateTime.Today);
        }

        private async Task OnDiaTocado(DateTime fecha)
        {
            var medicamentos = await App.Database.GetMedsByDateAsync(fecha);
            MedicamentosDiaSeleccionado = new ObservableCollection<Medicamento>(medicamentos);

            if (fecha.Date != DateTime.Today)
            {
                if (medicamentos.Count == 0)
                    await Application.Current.MainPage.DisplayAlert("Sin registro", "No hay medicamentos en este día.", "OK");
            }
        }

        private async void CargarEventos()
        {
            var medicamentos = await App.Database.GetMedsMesActualAsync();
            Events.Clear();

            foreach (var medicamento in medicamentos)
            {
                var fecha = medicamento.CreatedAt.Date;
                if (!Events.ContainsKey(fecha))
                    Events[fecha] = new List<Medicamento>();
                (Events[fecha] as List<Medicamento>)!.Add(medicamento);
            }
        }
    }
}
