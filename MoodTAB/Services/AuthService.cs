using System.Net.Http.Json;
using System.Text.Json;
using MoodTAB.ViewModels; // si quieres pasar logs al ViewModel

namespace MoodTAB.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://10.0.2.2:5051/api/") // emulador Android
            };
        }

        public async Task<(bool success, string log, PacienteDto user)> LoginAsync(string nombre, string email)
        {
            string log = "";

            try
            {
                log += $"Intentando login con Nombre={nombre}, Email={email}\n";

                var payload = new { Nombre = nombre, Email = email };
                var jsonPayload = JsonSerializer.Serialize(payload);
                log += $"JSON enviado: {jsonPayload}\n";

                var response = await _httpClient.PostAsJsonAsync("autenticacionlogin/login", payload);
                log += $"StatusCode: {response.StatusCode}\n";

                var responseBody = await response.Content.ReadAsStringAsync();
                log += $"Respuesta del backend: {responseBody}\n";

                if (!response.IsSuccessStatusCode)
                    return (false, log, null);

                var result = JsonSerializer.Deserialize<LoginResponse>(responseBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                log += $"Resultado Success: {result?.Success}, Nombre={result?.User?.Nombre}\n";

                if (result != null && result.Success)
                {
                    await SecureStorage.SetAsync("user_id", result.User.ID_Paciente.ToString());
                    await SecureStorage.SetAsync("user_nombre", result.User.Nombre);
                    await SecureStorage.SetAsync("user_email", result.User.Email);

                    return (true, log, result.User);
                }

                return (false, log, null);
            }
            catch (Exception ex)
            {
                log += $"Excepción: {ex.Message}\n";
                return (false, log, null);
            }
        }

        public async Task LogoutAsync()
        {
            SecureStorage.Remove("user_id");
            SecureStorage.Remove("user_nombre");
            SecureStorage.Remove("user_email");

            Application.Current.MainPage = new NavigationPage(new Vistas.LoginPage());
        }
    }

    public class LoginResponse
    {
        public bool Success { get; set; }
        public PacienteDto User { get; set; }
    }

    public class PacienteDto
    {
        public int ID_Paciente { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
    }
}
