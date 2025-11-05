namespace MoodTAB.Services
{

    public interface IHealthDataService
    {
        int TotalSteps { get; }
        int HeartRate { get; }
        int HRV { get; }
        double SleepHours { get; }
        double DistanceKm { get; }
        Task<bool> InitializeAndRequestPermissionsAsync();
        Task LoadAllHealthDataAsync();
    }
}