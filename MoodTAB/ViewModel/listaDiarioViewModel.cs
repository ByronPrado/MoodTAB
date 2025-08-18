using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoodTAB.Models;
using MoodTAB.Vistas;
using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Net.Http;

namespace MoodTAB.ViewModel
{
    public partial class ListaDiarioViewModel : ObservableObject
    {
        [ObservableProperty]
        public ObservableCollection<Diario> listaDiarios = [];
        public ListaDiarioViewModel()
        {
        }
        public async Task CargarListaDiarios()
        {
            try
            {
                var todas = await App.Database.GetDiarioAsync();
                ListaDiarios = new ObservableCollection<Diario>(todas);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar la lista de diarios: {ex.Message}");
            }
        }
    }
    
    
}