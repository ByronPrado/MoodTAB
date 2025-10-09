//using Foundation;
using System;
using System.Collections.Generic;


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
        public static bool esFamiliar { get; set; } = false;
        public static string? id_usuario_externo_DB { get; set; }

        public static string direccion_ngrok = "http://10.0.2.2:5051/";
        //public static string direccion_ngrok = "https://ed2fa62ed89e.ngrok-free.app/";

        public static bool toBool(string? boole)
        {
            return !string.IsNullOrEmpty(boole) && bool.TryParse(boole, out var result) && result;

        }

        public static Dictionary<string, string> colores = new Dictionary<string, string>
        {
            { "Feliz", "#fff692"},
            { "Emocionado", "#ffd195" },
            { "Cansado", "#cb9bff" },
            { "Triste", "#99c5fe" },
            { "Frustrado", "#e5fb96" },
            { "Enojado", "#ff8787" },
            { "Neutro", "#bababa" },
            { "Angustia", "#9baffd" },
            { "Ansioso", "#9bffe9" },
        };
        public static Dictionary<string, string> bordes = new Dictionary<string, string>
        {
            { "Feliz", "#e0d269"},
            { "Emocionado", "#d8a366" },
            { "Cansado", "#9866d1" },
            { "Triste", "#6a99d2" },
            { "Frustrado", "#a0cf62" },
            { "Enojado", "#cb6464" },
            { "Neutro", "#888888" },
            { "Angustia", "#6991cd" },
            { "Ansioso", "#5bbfab" },
        };
        public static Dictionary<string, string> emoticonos = new Dictionary<string, string>
        {
            { "Feliz", "😊"},
            { "Emocionado", "😃" },
            { "Cansado", "😪" },
            { "Triste", "😢" },
            { "Frustrado", "😖" },
            { "Enojado", "😠" },
            { "Neutro", "😑" },
            { "Angustia", "😰" },
            { "Ansioso", "🫨" },
        };

        public static Dictionary<string, ConsejoInfo> consejos = new Dictionary<string, ConsejoInfo>
        {
            { "1", new ConsejoInfo("Monitorea tus horas de sueño", "Cualquier cambio en la rutina de sueño puede estar ligada a cambios de -----", true) },
            { "2", new ConsejoInfo("Monitorea tu estado de animo", "Mantener un registro te ayudará a notar tus cambios --------", true) },
            { "3", new ConsejoInfo("Monitorea tus estado de animo", "Puedes escribir más de un diario al día", true)},
            { "4", new ConsejoInfo("titulo_4", "Contenido_4", true) },
            { "5", new ConsejoInfo("titulo_5", "Contenido_4", true) },
            { "6", new ConsejoInfo("titulo_6", "Contenido_4", true) },
            { "7", new ConsejoInfo("titulo_false", "Contenido_false", false) },

        };
        // Clase para representar los consejos (no estática, con constructor y propiedades)
        public class ConsejoInfo
        {
            public string Titulo { get; set; }
            public string Contenido { get; set; }
            public bool Util { get; set; }

            public ConsejoInfo(string titulo, string contenido, bool util)
            {
                Titulo = titulo;
                Contenido = contenido;
                Util = util;
            }

        }
    }
}
