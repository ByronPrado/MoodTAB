using Android.OS;
using MoodTAB.Services;
using System;

[assembly: Microsoft.Maui.Controls.Dependency(typeof(MoodTAB.Platforms.Android.DeviceUptimeService))]
namespace MoodTAB.Platforms.Android
{
    public class DeviceUptimeService : IDeviceUptimeService
    {
        public TimeSpan GetUptime()
        {
            long uptimeMillis = SystemClock.ElapsedRealtime();
            return TimeSpan.FromMilliseconds(uptimeMillis);
        }
    }
}
