using OpenAI.Chat;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq; 
using System;
using Microsoft.Extensions.Options; // Necesario si quieres inyectar opciones aquí, aunque mejor en Program.cs.

namespace WebConTablas.Controllers
{
    public class ChatService
    {
       private readonly ChatClient _client;
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
            
            // Asignamos los servicios inyectados
            _analyzer = analyzer;
            _detector = detector; 
        }

        public async Task<MessageAnalysisResult> SendMessageAsync(string userMessage, double timeElapsedSeconds)
        {
            // 1. Tarea de Detección de Palabras Sensibles (FuzzySharp)
            var detectedWords = _detector.Detect(userMessage);

            // 2. Tarea de Análisis Gramatical (IA)
            var grammarTask = GetGrammarErrorCountAsync(userMessage);

            // 3. Tarea de Respuesta del Bot (IA)
            _conversationHistory += $"Usuario: {userMessage}\n";
            var chatTask = _client.CompleteChatAsync(_conversationHistory);

            // 4. Esperamos todas las tareas
            await Task.WhenAll(grammarTask, chatTask);
            
            // 5. Obtenemos los resultados
            int actualGrammarErrors = grammarTask.Result;
            string botResponse = chatTask.Result.Value.Content[0].Text;
            
            // 6. Análisis General del Mensaje (WPM, Keywords, Coherence)
            var analysisResult = _analyzer.Analyze(userMessage, timeElapsedSeconds, actualGrammarErrors); 
            
            // 7. Finalizamos el historial de conversación (solo para la respuesta del bot)
            _conversationHistory += $"Asistente: {botResponse}\n";

            // 8. Añadimos las palabras sensibles REALES al resultado
            analysisResult.SensitiveWords.AddRange(detectedWords);

            // 9. Devolvemos el resultado
            analysisResult.Reply = botResponse;
            return analysisResult;
        }

        /// <summary>
        /// Solicita al modelo de IA que evalúe el texto en busca de errores gramaticales.
        /// </summary>
        private async Task<int> GetGrammarErrorCountAsync(string text)
        {
            var systemInstruction = "Eres un corrector de gramática. Analiza el siguiente texto. Tu única respuesta debe ser un número entero: la cantidad total de errores gramaticales detectados. Si no hay errores, responde 0.";
            
            // ❌ CORREGIDO: Se reemplaza ChatRequestMessage/System/User por ChatMessage/System/UserChatMessage 
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
                // Log el error (opcional)
                Console.WriteLine($"Error al obtener el conteo de errores gramaticales: {ex.Message}");
            }

            return 0; 
        }
    }
    
    // --- CLASE SIMULADA PARA ANÁLISIS ---
    public class MessageAnalyzer
    {
        // 🎯 CORRECCIÓN: Se reemplazan las listas codificadas por campos readonly sin inicialización.
        private readonly List<string> _sampleKeywords;
        private readonly List<string> _sampleFillerWords;

        // 🎯 CORRECCIÓN: El constructor recibe la configuración.
        public MessageAnalyzer(IOptions<AnalysisSettings> settings)
        {
            // Inicializa las listas usando la configuración inyectada
            _sampleKeywords = settings.Value.Keywords;
            _sampleFillerWords = settings.Value.FillerWords;
        }

        public MessageAnalysisResult Analyze(string message, double timeElapsedSeconds, int grammarErrorCount)
        {
            var result = new MessageAnalysisResult();
            string normalizedMessage = message.ToLowerInvariant();

            // 1) Palabras clave (Simulación: búsqueda simple)
            foreach (var keyword in _sampleKeywords) // 🎯 Ahora usa la lista inyectada
            {
                if (normalizedMessage.Contains(keyword))
                {
                    result.Keywords.Add(keyword);
                }
            }

            // 2) Errores gramaticales (del resultado de la IA)
            result.GrammaticalErrors = grammarErrorCount;

            // 3) Coherencia 
            result.CoherenceScore = grammarErrorCount == 0 ? "Alta" : (grammarErrorCount == 1 ? "Media" : "Baja");

            // 4) Velocidad de escritura (Calculado por WPM)
            int wordCount = message.Split(new char[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;
            if (timeElapsedSeconds > 0)
            {
                double timeMinutes = timeElapsedSeconds / 60.0;
                result.TypingSpeed_WPM = Math.Round(wordCount / timeMinutes, 2);
            }

            // 5) Identificar y contabilizar muletillas 
            var fillerCount = new Dictionary<string, int>();
            // 🎯 CORRECCIÓN: _sampleFillerWords es ahora una lista, no un diccionario.
            foreach (var filler in _sampleFillerWords) 
            {
                int count = 0;
                int index = normalizedMessage.IndexOf(filler);
                while (index != -1)
                {
                    // Contar solo si la muletilla es una palabra completa (o al menos un token)
                    // Este es un refinamiento de la lógica simple de IndexOf.
                    // Para ser más preciso, se podría verificar si antes y después hay espacios o puntuación.
                    count++;
                    index = normalizedMessage.IndexOf(filler, index + filler.Length);
                }
                if (count > 0)
                {
                    fillerCount.Add(filler, count);
                }
            }
            result.FillerWords = fillerCount;

            return result;
        }
    }
    
    // Nota: MessageAnalysisResult y SensitiveWordDetector deben estar definidos en otra parte
    // o en este mismo archivo si deseas que compile.
}