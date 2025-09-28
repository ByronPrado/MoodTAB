using CommunityToolkit.Mvvm.ComponentModel; 
using CommunityToolkit.Mvvm.Input;
using MoodTAB.Models;
using MoodTAB.Vistas;
using System.Collections.ObjectModel;
using Plugin.Maui.Calendar.Models;
using System.Globalization;
using System.Linq;
using Plugin.Maui.Calendar.Models;     
using Microsoft.Maui.Graphics;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Maui.Controls;

namespace MoodTAB.ViewModel
{
    public partial class ListaDiarioViewModel : ObservableObject
    {
        [ObservableProperty]
        public ObservableCollection<Diario> listaDiarios = [];
        [ObservableProperty]
        public DateTime fecha;

        public ListaDiarioViewModel()
        {
        }
        public async Task CargarListaDiarios(DateTime fecha)
        {
            try
            {
                Fecha = fecha.Date;
                var diariosDelDia = await App.Database.GetDiariosByDateAsync(fecha.Date);

                ListaDiarios = new ObservableCollection<Diario>(diariosDelDia);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar la lista de diarios: {ex.Message}");
            }
        }

        
    }    
}