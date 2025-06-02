using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp1;
using MoodTAB.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MoodTAB.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        // 1) Colección de ítems vinculada a la CollectionView
        [ObservableProperty]
        private ObservableCollection<TodoItem> items = new();

        // 2) Texto ingresado en el Entry
        [ObservableProperty]
        private string nuevaPersona;

        // 3) Título enlazado al Label
        [ObservableProperty]
        private string _title = "MoodTAB";

        // Propiedad para el RUT ingresado en el Entry
        [ObservableProperty]
        private string nuevoRut;

        //Lista temporal 

        public MainViewModel()
        {
            Items = new ObservableCollection<TodoItem>();
            NuevaPersona = string.Empty;
            NuevoRut = string.Empty;
        }

        // 4) Comando que carga todos los elementos desde SQLite
        [RelayCommand]
        private async Task LoadItems()
        {
            var itemsFromDb = await App.Database.GetItemsAsync();
            Items.Clear();
            foreach (var item in itemsFromDb)
            {
                Items.Add(item);
            }
        }

        // 5) Comando que añade un nuevo elemento a la BD 
        [RelayCommand]
        private async Task AddItem()
        {
            if (string.IsNullOrWhiteSpace(NuevaPersona))
                return;
            if (string.IsNullOrWhiteSpace(NuevoRut))
                return;

            var todoItem = new TodoItem
            {
                Nombre = NuevaPersona,
                Rut = NuevoRut,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                IsDone = false
            };

            await App.Database.SaveItemAsync(todoItem);

            // Limpiar el Entry
            NuevaPersona = string.Empty;
            NuevoRut = string.Empty;

            // Volver a cargar la lista
            await LoadItems();
        }
        
        [RelayCommand]
        private async Task DatabasePage()
        {
            if (Application.Current?.MainPage?.Navigation != null)
            {
                await Application.Current.MainPage.Navigation.PushAsync(new DataBasePage());
            }
            else
            {
                // Handle the case where navigation is not available
                // For example, show an alert or log an error
            }
        }
    }
}
