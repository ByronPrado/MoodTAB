namespace MoodTAB.Models
{
    public class EmocionItem
    {
        public string Texto { get; set; } = "Texto";
        public string Emoticon { get; set; } = "🤡";
        public string Color { get; set; } = "Red";
        public string ColorBorde { get; set; } = "Red";

        public EmocionItem(string texto, string emoticon, string color, string colorBorde)
        {
            Texto = texto;
            Emoticon = emoticon;
            Color = color;
            ColorBorde = colorBorde;
        }
        public EmocionItem(string texto, string color, string colorBorde)
        {
            Texto = texto;
            Color = color;
            ColorBorde = colorBorde;
        }
    }
}
