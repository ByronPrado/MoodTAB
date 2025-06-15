using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoodTAB.Models;
using MoodTAB.Vistas;
using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MoodTAB.Services;

namespace MoodTAB.ViewModel

{
    public partial class DiarioViewModel : ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<Diario> diarios = new();

        [ObservableProperty]
        string emocionDiaria;

        [ObservableProperty]
        string descDia;

        [ObservableProperty]
        double horasCelular;

        [ObservableProperty]
        int horasRedes;

        [ObservableProperty]
        int cantidadPasos;

        [ObservableProperty]
        string horasSueno;

        [ObservableProperty]
        bool anotado;

        [ObservableProperty]
        string error;
        public DiarioViewModel()
        {
            EmocionDiaria = "Seleccione su Emoción";
            DescDia = "";
            HorasCelular = 0.0;
            HorasRedes = 0;
            CantidadPasos = 0;
            HorasSueno = "0";
            Error = "";
            anotado = false;

            var uptime = DependencyService.Get<IDeviceUptimeService>()?.GetUptime();
            if (uptime != null)
            {
                // Función sin poder probar
                HorasCelular = uptime.Value.TotalHours;
            }
            else
            {
                HorasCelular = 1.0;
            }

            _ = LoadDiariosAsync();
        }

        [RelayCommand]
        private void SeleccionarEmocion(string emocion)
        {
            EmocionDiaria = emocion;
        }

        partial void OnHorasSuenoChanged(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                HorasSueno = new string(value.Where(char.IsDigit).ToArray());
            }
        }

        private async Task LoadDiariosAsync()
        {
            var items = await App.Database.GetDiarioAsync();
            Diarios = new ObservableCollection<Diario>(items);
        }

        [RelayCommand]
        private async Task GuardarDiario()
        {
            try
            { 
                if (Anotado)
                {
                    await Shell.Current.DisplayAlert("Diaro ya subido", "Usted ya subió su diario emocional hoy", "OK");
                    return;
                }
                if (string.IsNullOrWhiteSpace(EmocionDiaria) || string.IsNullOrWhiteSpace(DescDia))
                {
                    await Shell.Current.DisplayAlert("Campos en blanco", "No se puede dejar los campos en blanco", "OK");
                    return;
                }
                var diario = new Diario
                {
                    Emocion_Diaria = EmocionDiaria.Split(" ", 2)[1],
                    Descripcion = DescDia,
                    Horas_Celular = HorasCelular,
                    Horas_Redes = HorasRedes,
                    Horas_Sueno = HorasSueno,
                    Cantidad_Pasos = CantidadPasos,
                    CreatedAt = DateTime.Now,

                };

                await App.Database.SaveDiarioAsync(diario);
                Anotado = true;
                await LoadDiariosAsync();
            }
            catch (Exception e)
            {
                Error = e.Message;
            }
            
        }

        [RelayCommand]
        private async Task BorrarDiario()
        {
            try
            {
                var listadiarios = Diarios.ToList();
                foreach (Diario diario in listadiarios)
                {
                    await App.Database.DeleteDiarioAsync(diario);
                }
                await LoadDiariosAsync();
            }
            catch (Exception e)
            {
                Error = e.Message;
            }
        }
        
        [RelayCommand]
        private void CambiarAnotado()
        {
            if (Anotado)
            {
                Anotado = false;
            }
            else
            {
                Anotado = true;
            }
        }
    }
}