using OpenAI.Chat;

namespace WebConTablas.Controllers
{
    public class ChatService
    {
        private readonly ChatClient _client;
        private string _conversationHistory;

        public ChatService(string apiKey)
        {
            _client = new ChatClient(model: "gpt-4o-mini", apiKey: apiKey);
            _conversationHistory = "Eres un asistente amigable en español.\n";
        }

        public async Task<string> SendMessageAsync(string userMessage)
        {
            _conversationHistory += $"Usuario: {userMessage}\n";

            var completion = await _client.CompleteChatAsync(_conversationHistory);

            string botResponse = completion.Value.Content[0].Text;

            _conversationHistory += $"Asistente: {botResponse}\n";

            return botResponse;
        }
    }
}
