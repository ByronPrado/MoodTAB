using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoodTAB.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MoodTAB.ViewModel
{
    public partial class DataBaseViewModel : ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<TodoItem> items = new();
        [ObservableProperty]
        private string nuevaPersona;
        // Propiedad para el RUT ingresado en el Entry
        [ObservableProperty]
        private string nuevoRut;
        [ObservableProperty]
        private TodoItem? selectedItem;

        public DataBaseViewModel()
        {
           
            Task.Run(LoadItems);
            NuevaPersona = string.Empty;
            NuevoRut = string.Empty;
        }

        private async Task LoadItems()
        {
            var result = await App.Database.GetItemsAsync();
            Items = new ObservableCollection<TodoItem>(result);
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
        private async Task DeleteDoneItems()
        {
            // 1) Obtener todos los ítems marcados como IsDone == true
            var doneItems = Items.Where(x => x.IsDone == true).ToList();

            // 2) Para cada uno, borrarlo de la base de datos
            foreach (var item in doneItems)
            {
                await App.Database.DeleteItemAsync(item);
            }

            // 3) Recargar la lista desde la BD (se vaciará el contenido eliminado)
            await LoadItems();
        }

        [RelayCommand]
        private async Task SaveItem()
        {
            if (SelectedItem != null)
            {
                SelectedItem.UpdatedAt = DateTime.Now;
                await App.Database.SaveItemAsync(SelectedItem);
                await LoadItems(); // Actualizar la vista
                SelectedItem = null; // Limpiar la selección después de guardar
            }
        }
    }
}