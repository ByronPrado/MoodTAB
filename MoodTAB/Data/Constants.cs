using System.IO;
//using Xamarin.Essentials;

namespace MoodTAB.Data
{
    public static class Constants
    {
        public const string DatabaseFilename = "MoodTAB.db3";
        public static string direccion_local = "http://10.0.2.2:5051/";
        public static string direccion_ngrok = "https://ed2fa62ed89e.ngrok-free.app/";

        public static string DatabasePath
        {
            get
            {
                var path = FileSystem.AppDataDirectory;
                return Path.Combine(path, DatabaseFilename);
            }
        }

        public const string TodoItemTableName = "TodoItem";
        public const string PreguntaTableName = "Pregunta";
        public const string RespuestasTableName = "Respuestas";
        public const string DiarioTableName = "Diario";
    }
}