using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoodTAB.Vistas;
using Microsoft.Maui.Controls;


namespace MoodTAB.ViewModel
{
    public partial class UserViewModel : ObservableObject
    {
        [ObservableProperty] string nombre;
        [ObservableProperty] string email;
        [ObservableProperty] string telefono;
        [ObservableProperty] bool isEditing;
        [ObservableProperty] string botonEditar;
        //[ObservableProperty] string botonPlanSeguro;

        public UserViewModel()
        {
            // Inicializa con los datos actuales del usuario
            Nombre = SecureStorage.GetAsync("user_nombre").Result ?? "nombre test";
            Email = SecureStorage.GetAsync("user_email").Result ?? "email test";
            Telefono = "8888888";
            IsEditing = false;
            BotonEditar = "✏️ Editar";
            //BotonPlanSeguro = "📝 Editar Plan Seguro";
        }
//📝
        [RelayCommand]
        public void Editar()
        {
            if (!IsEditing)
            {
                IsEditing = true;
                BotonEditar = "Cancelar";
            }
            else
            {
                Nombre = SecureStorage.GetAsync("user_nombre").Result ?? "nombre test";
                Email = SecureStorage.GetAsync("user_email").Result ?? "email test";
                Telefono = "8888888";
                IsEditing = false;
                BotonEditar = "✏️ Editar";
            }

        }


        [RelayCommand]
        public async Task GuardarCambios()
        {
            var httpClient = new HttpClient();
            var url = $"{Globals.direccion_ngrok}api/apipacientesedit/" + SecureStorage.GetAsync("user_id").Result;
            var payload = new
            {
                nombre = Nombre,
                email = Email,
                telefono = Telefono
            };

            var json = System.Text.Json.JsonSerializer.Serialize(payload);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await httpClient.PatchAsync(url, content);
            var main = Microsoft.Maui.Controls.Application.Current?.MainPage;
            if (main == null) return;

            if (response.IsSuccessStatusCode)
            {
                //Globals.nombre_Usuario = Nombre;
                //Globals.email_Usuario = Email;
                await SecureStorage.SetAsync("user_nombre", Nombre);
                await SecureStorage.SetAsync("user_email", Email);

                await main.DisplayAlert("Cambios Guardados", "Los cambios se han guardado correctamente.", "OK");
            }
            else
            {
                await main.DisplayAlert("Error", "No se pudo guardar los cambios.", "OK");
            }

            IsEditing = false;
            BotonEditar = " Editar";
        }
    }
}