using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Maui.Controls;

namespace MoodTAB.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<(bool success, string log, PacienteDto user)> LoginAsync(string email, string contrasena,bool esFamiliar)
        {
            string log = "";

            try
            {
                var baseUrl = $"{Globals.direccion_ngrok}api/";
                _httpClient.BaseAddress = new Uri(baseUrl); // 🔹 se actualiza cada vez
                
                log += $"Intentando login con Email={email}\n";
                Console.WriteLine($"Log:{log}");
                var payload = new { Email = email , Password = contrasena , EsFamiliar = esFamiliar};
                var jsonPayload = JsonSerializer.Serialize(payload);
                log += $"JSON enviado: {jsonPayload}\n";
                Console.WriteLine($"Log:{log}");


                var response = await _httpClient.PostAsJsonAsync("autenticacionlogin/login", payload);
                log += $"StatusCode: {response.StatusCode}\n";

                Console.WriteLine($"Log:{log}");

                var responseBody = await response.Content.ReadAsStringAsync();
                log += $"Respuesta del backend: {responseBody}\n";
                Console.WriteLine($"Log:{log}");

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
                    await SecureStorage.SetAsync("psiquiatra_id", result.User.ID_Psiquiatra.ToString());
                    await SecureStorage.SetAsync("es_familiar", esFamiliar.ToString());

                    return (true, log, result.User);
                }

                return (false, log, null);
            }
            catch (Exception ex)
            {
                log += $"Excepción: {ex.Message}\n";
                Console.WriteLine($"Log:{log}");

                return (false, log, null);
            }
        }

        public async Task LogoutAsync()
        {
            SecureStorage.Remove("user_id");
            SecureStorage.Remove("user_nombre");
            SecureStorage.Remove("user_email");
            SecureStorage.Remove("psiquiatra_id");
            SecureStorage.Remove("es_familiar");

            Microsoft.Maui.Controls.Application.Current.MainPage = new NavigationPage(new Vistas.LoginPage());
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
        public int ID_Psiquiatra { get; set; }
        public int? IdUsuarioExterno { get; set; } // si aplica

    }
}
