using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MoodTAB.ViewModel
{
    public partial class UserViewModel : ObservableObject
    {
        [ObservableProperty] string nombre;
        [ObservableProperty] string email;
        [ObservableProperty] string telefono;
        [ObservableProperty] bool isEditing;


        public UserViewModel()
        {
            // Inicializa con los datos actuales del usuario
            Nombre = SecureStorage.GetAsync("user_nombre").Result?? "nombre test";
            Email = SecureStorage.GetAsync("user_email").Result ?? "email test";
            Telefono = "8888888";
            IsEditing = false;
        }

        [RelayCommand]
        public void Editar()
        {
            IsEditing = true;
        }

        [RelayCommand]
        public async Task GuardarCambios()
        {
            // Actualiza los datos globales y SecureStorage si es necesario
            Globals.nombre_Usuario = Nombre;
            Globals.email_Usuario = Email;
            //ahora vamos a guardar en la base de datos
            //conectamos a la api lueog esto

            await SecureStorage.SetAsync("user_nombre", Nombre);
            await SecureStorage.SetAsync("user_email", Email);

            await Shell.Current.DisplayAlert("Cambios Guardados", "Los cambios se han guardado correctamente.", "OK");
            IsEditing = false;
        }
    }
}