using FuzzySharp;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using WebConTablas.Controllers;

public class SensitiveWordDetector
{
    private readonly List<string> _sensitiveWords;
    private const int PhraseThreshold = 80; // Alto umbral para frases (casi exacto)
    private const int WordThreshold = 60;   // Umbral medio para palabras (permite errores)

    public SensitiveWordDetector(IOptions<AnalysisSettings> settings)
    {
        _sensitiveWords = settings.Value.SensitiveWords;
    }

    public List<string> Detect(string text)
    {
        var detected = new List<string>();
        string normalizedText = text.ToLowerInvariant();
        
        // 1. Detección de FRASES SENSIBLES (más larga y compleja)
        // Usamos PartialRatio para ver si alguna frase sensible está contenida en el texto, 
        // incluso si el usuario añade palabras antes o después (ej. "Yo no, pero me quiero morir ya").

        var sensitivePhrases = _sensitiveWords.Where(w => w.Contains(' ')).ToList();
        
        foreach (var phrase in sensitivePhrases)
        {
            // PartialRatio: Compara si la subcadena más similar al target excede el umbral.
            int score = Fuzz.PartialRatio(normalizedText, phrase);
            
            if (score >= PhraseThreshold)
            {
                // Si detectamos una frase, añadimos la frase maestra a la lista.
                detected.Add(phrase); 
            }
        }

        // 2. Detección de PALABRAS SENSIBLES INDIVIDUALES (con errores tipográficos)
        // Tokenizamos el texto del usuario para aislar cada palabra.
        // Se recomienda tokenizar mejor que solo por espacio para manejo de puntuación.
        var userWords = text.Split(new char[] { ' ', '.', ',', '?', '!', '¿', '¡', ':', ';' }, StringSplitOptions.RemoveEmptyEntries)
                              .Select(w => w.ToLowerInvariant()).ToList();
        
        var sensitiveSingleWords = _sensitiveWords.Where(w => !w.Contains(' ')).ToList();
        
        foreach (var userWord in userWords)
        {
            foreach (var sensitiveWord in sensitiveSingleWords)
            {
                // Ratio: Compara la palabra del usuario con la palabra sensible.
                // Es ideal para detectar errores ortográficos.
                int score = Fuzz.Ratio(userWord, sensitiveWord);
                
                if (score >= WordThreshold) 
                {
                    // Si encontramos una coincidencia con alta similitud
                    detected.Add(sensitiveWord); 
                }
            }
        }
        
        // Retornamos todas las detecciones únicas (tanto frases como palabras)
        return detected.Distinct().ToList();
    }
}