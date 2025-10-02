using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebConTablas.Controllers
{
    // Usamos [Controller] ya que estás devolviendo una View, pero la inyección es la clave.
    public class ChatController : Controller 
    {
        // 1. Campo de servicio READONLY inyectado
        private readonly ChatService _chatService;
        
        // ⚠️ El campo estático _lastAnalysis se mantiene para simplificar la depuración.
        private static MessageAnalysisResult _lastAnalysis = new MessageAnalysisResult();

        // 🎯 CORRECCIÓN: Usamos la Inyección de Dependencias.
        // ASP.NET Core buscará el ChatService registrado en Program.cs y lo inyectará.
        public ChatController(ChatService chatService)
        {
            // Asigna la instancia del servicio inyectado.
            _chatService = chatService;
        }

        // GET /ChatPage/Index
        [HttpGet("/ChatPage/Index")]
        public IActionResult Index()
        {
            // Puedes pasar el último análisis a la vista para la tabla de debug si es necesario.
            // return View("~/Views/ChatPage/Index.cshtml", _lastAnalysis); 
            return View("~/Views/ChatPage/Index.cshtml");
        }

        // Nuevo endpoint para devolver el último análisis de debug (opcional, pero útil)
        [HttpGet("/chat/lastanalysis")]
        public IActionResult GetLastAnalysis()
        {
            return Json(_lastAnalysis);
        }

        // POST /chat/send
        [HttpPost("/chat/send")]
        public async Task<IActionResult> SendMessage([FromBody] UserMessageWithTime message)
        {
            if (message == null || string.IsNullOrWhiteSpace(message.Text))
                return BadRequest(new { error = "El mensaje no puede estar vacío." });

            // El ChatService (ahora inyectado y configurado) realiza todo el análisis
            var analysisResult = await _chatService.SendMessageAsync(message.Text, message.TimeElapsedSeconds);

            // Guardamos el último resultado para el historial de la tabla (debug)
            _lastAnalysis = analysisResult;

            // Devolvemos el objeto de resultado completo
            return Json(analysisResult);
        }

        // Nuevo DTO para recibir el mensaje y el tiempo transcurrido
        public class UserMessageWithTime
        {
            public string Text { get; set; }
            public double TimeElapsedSeconds { get; set; } // Tiempo transcurrido en segundos
        }
    }

    // Usamos el DTO de resultados que definimos anteriormente (debería estar accesible)
    // public class MessageAnalysisResult { ... }
    // Dentro del namespace WebConTablas.Controllers
    public class UserMessage
    {
        public string Text { get; set; }
    }

    public class MessageAnalysisResult
    {
        public string Reply { get; set; }
        public List<string> SensitiveWords { get; set; } = new List<string>();
        
        // Nuevos campos de análisis
        public List<string> Keywords { get; set; } = new List<string>(); // 1) Palabras clave
        public int GrammaticalErrors { get; set; } = 0; // 2) Cantidad de errores gramaticales
        public string CoherenceScore { get; set; } = "Alto"; // 3) Medidores de Coherencia
        public double TypingSpeed_WPM { get; set; } = 0.0; // 4) Velocidad de escritura (palabras por minuto)
        public Dictionary<string, int> FillerWords { get; set; } = new Dictionary<string, int>(); // 5) Muletillas (Diccionario: Muletilla, Cantidad)
    }
}