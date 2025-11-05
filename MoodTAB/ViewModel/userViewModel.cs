using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoodTAB.Vistas;
using Microsoft.Maui.Media;
using Microsoft.Maui.Controls;
using System.IO;


namespace MoodTAB.ViewModel
{
    public partial class UserViewModel : ObservableObject
    {
        [ObservableProperty] string nombre;
        [ObservableProperty] string email;
        [ObservableProperty] string telefono;
        [ObservableProperty] bool isEditing;
        [ObservableProperty] bool isNotEditing;
        [ObservableProperty] ImageSource imagenPerfil;

        private readonly string _imagePath;
        public UserViewModel()
        {
            Nombre = SecureStorage.GetAsync("user_nombre").Result ?? "nombre test";
            Email = SecureStorage.GetAsync("user_email").Result ?? "email test";
            Telefono = "8888888";
            IsEditing = false;
            IsNotEditing = true;
            _imagePath = Path.Combine(FileSystem.AppDataDirectory, "profile_image.jpg");
            LoadProfileImage();
        }
        
        private void LoadProfileImage()
        {
            if (File.Exists(_imagePath))
            {
                ImagenPerfil = ImageSource.FromFile(_imagePath);
            }
            else
            {
                ImagenPerfil = ImageSource.FromFile("dotnet_bot.jpg");
            }
        }
        //📝
        [RelayCommand]
        public void Editar()
        {
            if (!IsEditing)
            {
                IsEditing = true;
                IsNotEditing = false;
            }
        }

        [RelayCommand]
        public void Cancelar()
        {
            Nombre = SecureStorage.GetAsync("user_nombre").Result ?? "nombre test";
            Email = SecureStorage.GetAsync("user_email").Result ?? "email test";
            Telefono = "8888888";
            IsEditing = false;
            IsNotEditing = true;
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
            IsNotEditing = true;
        }

        [RelayCommand]
        public async Task CambiarFoto()
        {
            try
            {
                var main = Microsoft.Maui.Controls.Application.Current?.MainPage;

                string opcion = await main.DisplayActionSheet(
                    "Selecciona una opción",
                    "Cancelar",
                    null,
                    "Tomar foto",
                    "Elegir de galería");

                FileResult photo = null;

                if (opcion == "Tomar foto")
                {
                    photo = await MediaPicker.CapturePhotoAsync();
                }
                else if (opcion == "Elegir de galería")
                {
                    photo = await MediaPicker.PickPhotoAsync();
                }

                if (photo != null)
                {
                    // Guardar la imagen permanentemente
                    await SaveProfileImage(photo);
                    
                    // Actualizar la imagen mostrada
                    ImagenPerfil = ImageSource.FromFile(_imagePath);
                }
            }
            catch (Exception ex)
            {
                await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert(
                    "Error", $"No se pudo cambiar la foto: {ex.Message}", "OK");
            }
        }

        private async Task SaveProfileImage(FileResult photo)
        {
            using var stream = await photo.OpenReadAsync();
            using var newStream = File.Create(_imagePath);
            await stream.CopyToAsync(newStream);
        }
    }
}