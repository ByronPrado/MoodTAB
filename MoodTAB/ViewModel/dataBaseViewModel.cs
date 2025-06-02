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

        public DataBaseViewModel()
        {
            _ = LoadItems();
        }

        private async Task LoadItems()
        {
            var result = await App.Database.GetItemsAsync();
            Items = new ObservableCollection<TodoItem>(result);
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