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
            var httpClient = new HttpClient();
            var url = "http://10.0.2.2:5051/api/apipacientesedit/" + SecureStorage.GetAsync("user_id").Result;
            var payload = new
            {
                nombre = Nombre,
                email = Email,
                telefono = Telefono
            };

            var json = System.Text.Json.JsonSerializer.Serialize(payload);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await httpClient.PatchAsync(url, content);

            if (response.IsSuccessStatusCode)
            {   
                //Globals.nombre_Usuario = Nombre;
                //Globals.email_Usuario = Email;

                await SecureStorage.SetAsync("user_nombre", Nombre);
                await SecureStorage.SetAsync("user_email", Email);

                await Shell.Current.DisplayAlert("Cambios Guardados", "Los cambios se han guardado correctamente.", "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "No se pudo guardar los cambios.", "OK");
            }

            IsEditing = false;
        }
    }
}