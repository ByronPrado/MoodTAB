using Microsoft.AspNetCore.Mvc;

namespace WebConTablas.Controllers
{
    public class ChatController : Controller
    {
        private readonly ChatService _chatService;
        private readonly SensitiveWordDetector _detector;

        // ⚠️ Guardamos en memoria las palabras detectadas (debug)
        private static List<string> _detectedHistory = new();

        public ChatController(ChatService chatService)
        {
            _chatService = chatService;
            _detector = new SensitiveWordDetector();
        }

        // GET /ChatPage/Index
        [HttpGet("/ChatPage/Index")]
        public IActionResult Index()
        {
            return View("~/Views/ChatPage/Index.cshtml");
        }

        // POST /chat/send
        [HttpPost("/chat/send")]
        public async Task<IActionResult> SendMessage([FromBody] UserMessage message)
        {
            if (message == null || string.IsNullOrWhiteSpace(message.Text))
                return BadRequest(new { error = "El mensaje no puede estar vacío." });

            // 🔹 Detectar palabras sensibles en el mensaje
            var detected = _detector.Detect(message.Text);
            if (detected.Any())
            {
                _detectedHistory.AddRange(detected);
            }

            var response = await _chatService.SendMessageAsync(message.Text);

            // 🔹 Devolvemos respuesta del bot + lista de palabras sensibles
            return Json(new
            {
                reply = response,
                sensitiveWords = _detectedHistory.Distinct().ToList()
            });
        }
    }

    public class UserMessage
    {
        public string Text { get; set; }
        public List<string> DetectedWords { get; set; } = new List<string>();
    }
}
