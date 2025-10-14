using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebConTablas.Common; // <--- ¡Añadir este using es crucial para MessageAnalysisResult!
using WebConTablas.Controllers; 
// Nota: Si el controlador está en el mismo namespace que ChatService, se puede omitir el using WebConTablas.Controllers, pero lo mantendremos por seguridad.

namespace WebConTablas.Controllers
{
    // Usamos [Controller] ya que estás devolviendo una View.
    public class ChatController : Controller 
    {
        // 1. Campo de servicio READONLY inyectado
        private readonly ChatService _chatService;
        
        // ⚠️ El campo estático _lastAnalysis usa el tipo de WebConTablas.Common
        private static MessageAnalysisResult _lastAnalysis = new MessageAnalysisResult();

        // 🎯 CORRECCIÓN: Usamos la Inyección de Dependencias.
        public ChatController(ChatService chatService)
        {
            // Asigna la instancia del servicio inyectado.
            _chatService = chatService;
        }

        // GET /ChatPage/Index
        [HttpGet("/ChatPage/Index")]
        public IActionResult Index()
        {
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

        // Nuevo DTO para recibir el mensaje y el tiempo transcurrido (se mantiene localmente si solo se usa aquí)
        public class UserMessageWithTime
        {
            public string Text { get; set; }
            public double TimeElapsedSeconds { get; set; } // Tiempo transcurrido en segundos
        }
    }

    // ***************************************************************
    // *** CLASES DUPLICADAS ELIMINADAS ***
    // MessageAnalysisResult y UserMessage (innecesario) han sido ELIMINADAS
    // de este archivo para evitar conflictos con WebConTablas.Common
    // ***************************************************************
}