using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebConTablas.Controllers
{
    public class DiarioTestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(DiarioInput diario)
        {
            using var client = new HttpClient();
            var url = "http://localhost:5051/api/DiarioEmocional"; 
            var json = JsonSerializer.Serialize(diario);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);

            ViewBag.Resultado = await response.Content.ReadAsStringAsync();
            return View();
        }

        public class DiarioInput
        {
            public int ID_Paciente { get; set; }
            public string Emociones { get; set; }
            public int? Pasos { get; set; }
            public int? Horas_celular { get; set; }
            public int? Horas_redes { get; set; }
            public string Hora_dormida { get; set; }
            public string Fecha { get; set; }
        }
    }
}