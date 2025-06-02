namespace MoodTAB;
using MoodTAB.Data;

public partial class App : Application
{
    static TodoItemDataBase? database;

        public static TodoItemDataBase Database
        {
            get
            {
                if (database == null)
                {
                    database = new TodoItemDataBase(Constants.DatabasePath);
                }
                return database;
            }
        }
    public App()
    {
        InitializeComponent();

        //MainPage = new NavigationPage(new MainPage());
        MainPage = new AppShell();
	}
}
