//using Foundation;
using System;
using System.Collections.Generic;
//

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

        //public static string direccion_ngrok = "http://10.0.2.2:5051/";
        public static string direccion_ngrok = "https://a28e8b3dc6bd.ngrok-free.app/";

        public static bool toBool(string? boole)
        {
            return !string.IsNullOrEmpty(boole) && bool.TryParse(boole, out var result) && result;

        }

        public static Dictionary<string, ConsejoInfo> consejos = new Dictionary<string, ConsejoInfo>
        {
            { "1", new ConsejoInfo("Monitorea tus horas de sueño", 
                "Cualquier cambio en la rutina de sueño puede estar ligado a cambios emocionales o físicos.", true) },

            { "2", new ConsejoInfo("Ponle nombre a lo que sientes", 
                "Decir 'estoy frustrado' o 'estoy triste' te ayuda a entenderte mejor.", true) },

            { "3", new ConsejoInfo("Recuerda que las emociones son pasajeras", 
                "No duran para siempre, aunque parezcan intensas en el momento.", true) },

            { "4", new ConsejoInfo("Evita juzgarte", 
                "Sentir demasiado o muy poco no te hace débil; te hace humano.", true) },

            { "5", new ConsejoInfo("Cuida tu cuerpo", 
                "Dormir, comer bien y moverte mejora tu regulación emocional.", true) },

            { "6", new ConsejoInfo("Respira antes de reaccionar", 
                "Tómate unos segundos para responder; no actúes en automático.", true) },

            { "7", new ConsejoInfo("Detente un minuto si sientes rabia", 
                "Aléjate o guarda silencio antes de actuar para evitar impulsividad.", true) },

            { "8", new ConsejoInfo("Pon paños fríos", 
                "Habla de temas neutros o respira hasta calmarte.", true) },

            { "9", new ConsejoInfo("Usa tu energía para crear", 
                "Canaliza la intensidad en arte, deporte o tareas concretas.", true) },

            { "10", new ConsejoInfo("Evita tomar decisiones en crisis", 
                "Espera a sentirte más estable antes de decidir.", true) },

            { "11", new ConsejoInfo("Habla en primera persona", 
                "Explica cómo te sientes sin culpar a otros. Ejemplo: 'Me dolió que…'", true) },

            { "12", new ConsejoInfo("Dale espacio a tus emociones", 
                "Evitar sentir solo las hace más fuertes después.", true) },

            { "13", new ConsejoInfo("Exprésate aunque cueste", 
                "Escribe, dibuja o habla con alguien de confianza.", true) },

            { "14", new ConsejoInfo("Reconoce tus logros", 
                "Aunque sean pequeños, refuerza lo que sí haces bien.", true) },

            { "15", new ConsejoInfo("Busca actividades que te conecten", 
                "Caminar, tejer, escuchar música o conversar ayudan a salir del bloqueo.", true) },

            { "16", new ConsejoInfo("Evita aislarte", 
                "Pedir ayuda no es signo de debilidad, sino de autocuidado.", true) },

            { "17", new ConsejoInfo("Valida tus emociones", 
                "Lo que sientes tiene sentido, aunque no lo entiendas del todo.", true) },

            { "18", new ConsejoInfo("Pide ayuda profesional si lo necesitas", 
                "Un psicólogo puede enseñarte estrategias para regularte.", true) },

            { "19", new ConsejoInfo("Rodéate de personas queridas", 
                "La conexión con otros calma y ayuda a regularte.", true) },

            { "20", new ConsejoInfo("Sé paciente contigo mismo", 
                "Cambiar requiere práctica, no perfección.", true) }

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

        public static bool OptionSueno { get; set; } = false;
        public static bool OptionHR { get; set; } = false;
        public static bool OptionHVR { get; set; } = false;
        public static bool OptionPasos { get; set; } = false;
        public static bool OptionManual { get; set; } = false;
    }
}
