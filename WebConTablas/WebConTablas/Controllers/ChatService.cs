using OpenAI.Chat;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq; 
using System;
using Microsoft.Extensions.Options;
using System.Globalization; 
// Nota: 'FuzzySharp' no es necesario aquí si solo se usa en SensitiveWordDetector
// pero no afecta si lo dejas.

using WebConTablas.Common; // <-- ¡CLAVE! Esto da acceso a MessageAnalyzer, SensitiveWordDetector, y MessageAnalysisResult.

namespace WebConTablas.Controllers
{
    // =========================================================================
    // === CHAT SERVICE (Solo Lógica del Chat) =================================
    // =========================================================================
    public class ChatService
    {
        private readonly ChatClient _client;
        // Estas clases se inyectan y provienen de WebConTablas.Common
        private readonly MessageAnalyzer _analyzer;
        private readonly SensitiveWordDetector _detector; 
        private string _conversationHistory;

        public ChatService(
            string apiKey, 
            MessageAnalyzer analyzer, 
            SensitiveWordDetector detector) 
        {
            _client = new ChatClient(model: "gpt-4o-mini", apiKey: apiKey);
            _conversationHistory = "Eres un asistente amigable en español. Mantén las respuestas breves.\n";
            _analyzer = analyzer;
            _detector = detector; 
        }

        public async Task<MessageAnalysisResult> SendMessageAsync(string userMessage, double timeElapsedSeconds)
        {
            var detectedWords = _detector.Detect(userMessage);
            var grammarTask = GetGrammarErrorCountAsync(userMessage);
            
            _conversationHistory += $"Usuario: {userMessage}\n";
            var chatTask = _client.CompleteChatAsync(_conversationHistory);

            await Task.WhenAll(grammarTask, chatTask);
            
            int actualGrammarErrors = grammarTask.Result;
            string botResponse = chatTask.Result.Value.Content[0].Text;
            
            // MessageAnalyzer y MessageAnalysisResult son de WebConTablas.Common
            var analysisResult = _analyzer.Analyze(userMessage, timeElapsedSeconds, actualGrammarErrors); 
            
            _conversationHistory += $"Asistente: {botResponse}\n";

            analysisResult.SensitiveWords.AddRange(detectedWords);
            analysisResult.Reply = botResponse;
            return analysisResult;
        }

        // --- MÉTODOS DE APOYO DE IA ---

        public async Task<int> GetGrammarErrorCountAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return 0;
            
            var systemInstruction = "Eres un corrector de gramática y ortografía EXTREMADAMENTE estricto. Revisa el texto e incluye errores leves como faltas de tilde o concordancia. Tu única respuesta DEBE SER SOLO el número entero de errores gramaticales y ortográficos detectados. Si no hay errores, el único valor es 0.";
            
            var analysisHistory = new List<ChatMessage>
            {
                new SystemChatMessage(systemInstruction),
                new UserChatMessage(text)
            };

            try
            {
                var completion = await _client.CompleteChatAsync(analysisHistory);
                string resultText = completion.Value.Content[0].Text.Trim();

                if (int.TryParse(resultText, out int errorCount))
                {
                    return errorCount;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el conteo de errores gramaticales: {ex.Message}");
            }
            return 0; 
        }

        public async Task<float> GetSemanticCoherenceAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return 1.0f;
            
            var systemInstruction = "Eres un evaluador de la coherencia del discurso. Analiza si el texto fluye lógicamente y se mantiene en un tema central, ignorando la gramática. Tu única respuesta DEBE SER SOLO un número flotante entre 0.0 y 2.0. Donde 2.0 es totalmente coherente (ideas bien conectadas) y 0.0 es totalmente incoherente (ideas que saltan sin relación).";
            
            var analysisHistory = new List<ChatMessage>
            {
                new SystemChatMessage(systemInstruction),
                new UserChatMessage(text)
            };

            try
            {
                var completion = await _client.CompleteChatAsync(analysisHistory);
                string resultText = completion.Value.Content[0].Text.Trim();

                if (float.TryParse(resultText.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out float coherenceScore))
                {
                    return Math.Clamp(coherenceScore, 0.0f, 2.0f);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener la puntuación de coherencia: {ex.Message}");
            }
            return 1.0f; 
        }
    }
}
// *** IMPORTANTE ***: Asegúrate de que NO haya ninguna definición de MessageAnalyzer, 
// MessageAnalysisResult, AnalysisSettings, o SensitiveWordDetector después de esta línea
// o dentro del namespace WebConTablas.Controllers.