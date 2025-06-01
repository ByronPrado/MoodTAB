using System.IO;
//using Xamarin.Essentials;

namespace MoodTAB.Data
{
    public static class Constants
    {
        public const string DatabaseFilename = "MoodTAB.db3";

        public static string DatabasePath
        {
            get
            {
                var path = FileSystem.AppDataDirectory;
                return Path.Combine(path, DatabaseFilename);
            }
        }

        public const string TodoItemTableName = "TodoItem";
    }
}