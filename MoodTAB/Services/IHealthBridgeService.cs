namespace MyMauiApp.Services
{
    public interface IHealthBridgeService
    {
        long GetStepsToday();
        long GetSleepMinutesToday();
    }
}