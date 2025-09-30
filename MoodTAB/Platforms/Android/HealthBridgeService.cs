using Android.Content;
//using Com.Example.Healthbridge; // generado automáticamente desde tu .aar
using MoodTAB.Services;
using MyMauiApp.Services;

namespace MoodTAB.Platforms.Android
{
    public class HealthBridgeService : IHealthBridgeService
    {
        private readonly Context _context;

        public HealthBridgeService(Context context)
        {
            _context = context;
        }

        public long GetStepsToday()
        {
            //return HealthBridge.GetStepsTodayBlocking(_context);
            return 0;
        }

        public long GetSleepMinutesToday()
        {
            //return HealthBridge.GetSleepMinutesTodayBlocking(_context);
            return 0;
        }
    }
}
