using FuzzySharp;

public class SensitiveWordDetector
{
    private readonly List<string> _sensitiveWords = new List<string>
    {
        // Suicidio
        "suicidio", "suicidarme", "suicidarse", "suicidarme yo", "intentar suicidio",
        "matarme", "matarse", "matarme yo", "quitarme la vida",
        "acabar con mi vida", "terminar con mi vida",

        // Muerte
        "muerte", "morir", "morirme", "morirse", "quiero morir",
        "me quiero morir", "deseo morir", "muerto", "muerta",

        // Depresión
        "depresión", "depresion", "depresivo", "depresiva",
        "estoy deprimido", "estoy deprimida", "me siento deprimido",
        "me siento deprimida", "sin ganas de vivir",

        // Autolesión
        "autolesión", "autolesion", "autolesiones", "autolesionarme",
        "lastimarme", "lastimarse", "hacerme daño", "hacerme daño a mí mismo",
        "hacerme daño a mi misma", "cortarme", "cortarme las venas",
        "cortarme yo", "autoagresión", "autoagresion", "autoagredirme",

        // Otros términos relacionados
        "ahorcarme", "ahorcarse", "tirarme por", "tirarme del puente",
        "tirarme por la ventana", "sobredosis", "tomar pastillas",
        "tomarme pastillas", "beber veneno", "veneno", "colgarme",
        "colgarse", "asfixiarme", "asfixiarse",
        "no quiero vivir", "no puedo más", "me quiero ir", "no aguanto más",
        "acabar conmigo", "terminar conmigo", "desaparecer", "desaparecerme",
        "estoy al límite", "estoy cansado de vivir", "estoy cansada de vivir"
    };

    public List<string> Detect(string text)
    {
        var detected = new List<string>();
        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        foreach (var word in words)
        {
            foreach (var sensitive in _sensitiveWords)
            {
                int score = Fuzz.Ratio(word.ToLower(), sensitive.ToLower());
                if (score > 50) // tolerancia, 100 = exacto
                {
                    detected.Add(sensitive);
                }
            }
        }

        return detected;
    }
}
