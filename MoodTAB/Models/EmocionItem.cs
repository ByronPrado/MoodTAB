namespace MoodTAB.Models
{
    public class EmocionItem
    {
        public string Texto { get; set; }
        public string Color { get; set; }
        public string ColorBorde { get; set; }
        public EmocionItem(string texto, string color, string colorBorde)
        {
            Texto = texto;
            Color = color;
            ColorBorde = colorBorde;
        }
    }
}
