using Microsoft.AspNetCore.Mvc;

namespace WebConTablas.Controllers
{
    public class ChatController : Controller
    {
        private readonly ChatService _chatService;

        public ChatController(ChatService chatService)
        {
            _chatService = chatService;
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

            var response = await _chatService.SendMessageAsync(message.Text);

            return Json(new { reply = response });
        }
    }

    public class UserMessage
    {
        public string Text { get; set; }
    }
}
