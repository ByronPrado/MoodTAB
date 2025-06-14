using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MoodTAB.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<string>> GetPreguntasAsync()
        {
            // Cambia la URL por la de tu backend (puede ser IP local o dominio)
            var url = "http://10.0.2.2:5051/api/preguntas";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
        }
    }
}