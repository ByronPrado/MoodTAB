namespace MoodTAB
{
    public static class Globals
    {
        public static string? nombre_Usuario { get; set; }
        public static string? email_Usuario { get; set; }
        public static string? id_paciente_DB { get; set; }
        public static string? cuestionario { get; set; }
        public static bool cuestionario_pendiente { get; set; } = false;
        public static bool respondido { get; set; } = false;

        public static bool toBool(string boole)
        {   
            if (boole == "true") return true;
            return false;
        }
    }
}
