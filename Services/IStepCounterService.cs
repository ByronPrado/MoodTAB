namespace MoodTAB.Services
{
    public interface IStepCounterService
    {
        void Start();
        void Stop();
        long TotalSteps { get; }
    }
}
