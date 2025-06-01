using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

        // 5) Comando que añade un nuevo elemento a la BD y recarga la lista
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
        private async Task DeleteDoneItems()
        {
            // 1) Obtener todos los ítems marcados como IsDone == true
            var doneItems = Items.Where(x => x.IsDone == false).ToList();

            // 2) Para cada uno, borrarlo de la base de datos
            foreach (var item in doneItems)
            {
                await App.Database.DeleteItemAsync(item);
            }

            // 3) Recargar la lista desde la BD (se vaciará el contenido eliminado)
            await LoadItems();
        }
    }
}
