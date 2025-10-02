namespace WebConTablas.Controllers
{
    public class AnalysisSettings
    {
        public List<string> Keywords { get; set; } = new List<string>();
        public List<string> FillerWords { get; set; } = new List<string>();
        public List<string> SensitiveWords { get; set; } = new List<string>();
        public string ApiKey { get; set; } // Opcional, si mapeas la clave de OpenAI aquí
    }
}