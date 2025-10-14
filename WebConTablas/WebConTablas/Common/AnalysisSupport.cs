using System.Collections.Generic;
using Microsoft.Extensions.Options;
using System.Linq;
using FuzzySharp;

namespace WebConTablas.Common
{
    // 1. CLASE MessageAnalyzer
    public class MessageAnalyzer
    {
        private readonly List<string> _sampleKeywords;
        private readonly List<string> _sampleFillerWords;

        public MessageAnalyzer(IOptions<AnalysisSettings> settings)
        {
            _sampleKeywords = settings.Value.Keywords;
            _sampleFillerWords = settings.Value.FillerWords;
        }

        public MessageAnalysisResult Analyze(string message, double timeElapsedSeconds, int grammarErrorCount)
        {
            var result = new MessageAnalysisResult();
            string normalizedMessage = message.ToLowerInvariant();

            foreach (var keyword in _sampleKeywords) 
            {
                if (normalizedMessage.Contains(keyword))
                {
                    result.Keywords.Add(keyword);
                }
            }

            result.GrammaticalErrors = grammarErrorCount;
            result.CoherenceScore = "N/A (Calculado por IA en DiarioEmocionalController)";

            int wordCount = message.Split(new char[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;
            if (timeElapsedSeconds > 0)
            {
                double timeMinutes = timeElapsedSeconds / 60.0;
                result.TypingSpeed_WPM = Math.Round(wordCount / timeMinutes, 2);
            }

            var fillerCount = new Dictionary<string, int>();
            foreach (var filler in _sampleFillerWords) 
            {
                int count = 0;
                int index = normalizedMessage.IndexOf(filler);
                while (index != -1)
                {
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

    // 2. CLASE SensitiveWordDetector
    public class SensitiveWordDetector
    {
        private readonly List<string> _sensitiveWords;
        private const int PhraseThreshold = 80;
        private const int WordThreshold = 60; 

        public SensitiveWordDetector(IOptions<AnalysisSettings> settings)
        {
            _sensitiveWords = settings.Value.SensitiveWords; 
        }

        public List<string> Detect(string text)
        {
            var detected = new List<string>();
            string normalizedText = text.ToLowerInvariant();
            
            var sensitivePhrases = _sensitiveWords.Where(w => w.Contains(' ')).ToList();
            
            foreach (var phrase in sensitivePhrases)
            {
                int score = Fuzz.PartialRatio(normalizedText, phrase);
                
                if (score >= PhraseThreshold)
                {
                    detected.Add(phrase); 
                }
            }

            var userWords = text.Split(new char[] { ' ', '.', ',', '?', '!', '¿', '¡', ':', ';' }, StringSplitOptions.RemoveEmptyEntries)
                                 .Select(w => w.ToLowerInvariant()).ToList();
            
            var sensitiveSingleWords = _sensitiveWords.Where(w => !w.Contains(' ')).ToList();
            
            foreach (var userWord in userWords)
            {
                foreach (var sensitiveWord in sensitiveSingleWords)
                {
                    int score = Fuzz.Ratio(userWord, sensitiveWord);
                    
                    if (score >= WordThreshold) 
                    {
                        detected.Add(sensitiveWord); 
                    }
                }
            }
            
            return detected.Distinct().ToList();
        }
    }

    // 3. CLASES DTO/SETTINGS
    public class AnalysisSettings
    {
        public List<string> Keywords { get; set; } = new List<string> { "ansiedad", "tristeza", "alegría" };
        public List<string> FillerWords { get; set; } = new List<string> { "esto es", "es decir", "pues" };
        public List<string> SensitiveWords { get; set; } = new List<string> { "suicidio", "morir", "autolesión", "no puedo más" }; 
    }
    
    public class MessageAnalysisResult
    {
        public string Reply { get; set; } = "";
        public List<string> Keywords { get; set; } = new List<string>();
        public int GrammaticalErrors { get; set; }
        public string CoherenceScore { get; set; } = "";
        public double TypingSpeed_WPM { get; set; }
        public Dictionary<string, int> FillerWords { get; set; } = new Dictionary<string, int>();
        public List<string> SensitiveWords { get; set; } = new List<string>();
    }
}