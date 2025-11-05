using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#if ANDROID
using AndroidX.Health.Connect.Client;
using AndroidX.Health.Connect.Client.Records;
using MoodTAB.Platforms.Android.Callbacks;
using MoodTAB.Platforms.Android.Permissions;
using AndroidX.Health.Connect.Client.Permission;
using Java.Util;
using Java.Time;
using Android.App;
using Android.Content;
#endif
namespace MoodTAB.Services
{
    public class HealthDataService : IHealthDataService
    {
#if ANDROID
        private KotlinCallback _healthConnectClient;
#endif
        public int TotalSteps { get; private set; }
        public int HeartRate { get; private set; }
        public int HRV { get; private set; }
        public double SleepHours { get; private set; }
        public double DistanceKm { get; private set; }

#if ANDROID
        private Instant DateTimeToInstant(DateTime date)
        {
            long unixTimestamp = ((DateTimeOffset)date).ToUnixTimeSeconds();
            return Instant.OfEpochSecond(unixTimestamp);
        }

        public async Task<bool> InitializeAndRequestPermissionsAsync()
        {
            try
            {
                var providerPackageName = "com.google.android.apps.healthdata";
                int availabilityStatus = HealthConnectClient.GetSdkStatus(Platform.CurrentActivity, providerPackageName);

                if (availabilityStatus != HealthConnectClient.SdkAvailable)
                    return false;

                var client = HealthConnectClient.GetOrCreate(Android.App.Application.Context);
                _healthConnectClient = new KotlinCallback(client);

                // Solicitar permisos
                var permissionsGranted = await RequestAllPermissions();
                return permissionsGranted;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> RequestAllPermissions()
        {
            try
            {
                var permissionsToGrant = new HashSet();
                
                var stepsRecord = new StepsRecord(DateTimeToInstant(DateTime.Now.AddMinutes(-1)), ZoneOffset.OfHours(0),
                    DateTimeToInstant(DateTime.Now), ZoneOffset.OfHours(0), 1, null);
                var sleepRecord = new SleepSessionRecord(DateTimeToInstant(DateTime.Now.AddHours(-8)), ZoneOffset.OfHours(0),
                    DateTimeToInstant(DateTime.Now.AddHours(-1)), ZoneOffset.OfHours(0), null, null, new List<SleepSessionRecord.Stage>(), null);
                var heartRateRecord = new HeartRateRecord(DateTimeToInstant(DateTime.Now.AddMinutes(-1)), ZoneOffset.OfHours(0),
                    DateTimeToInstant(DateTime.Now), ZoneOffset.OfHours(0), new List<HeartRateRecord.Sample>(), null);
                var distanceRecord = new DistanceRecord(DateTimeToInstant(DateTime.Now.AddMinutes(-1)), ZoneOffset.OfHours(0),
                    DateTimeToInstant(DateTime.Now), ZoneOffset.OfHours(0), AndroidX.Health.Connect.Client.Units.Length.InvokeMeters(1), null);
                var hrvClass = Java.Lang.Class.ForName("androidx.health.connect.client.records.HeartRateVariabilityRmssdRecord");

                permissionsToGrant.Add(HealthPermission.GetReadPermission(Kotlin.Jvm.Internal.Reflection.GetOrCreateKotlinClass(stepsRecord.Class)));
                permissionsToGrant.Add(HealthPermission.GetReadPermission(Kotlin.Jvm.Internal.Reflection.GetOrCreateKotlinClass(sleepRecord.Class)));
                permissionsToGrant.Add(HealthPermission.GetReadPermission(Kotlin.Jvm.Internal.Reflection.GetOrCreateKotlinClass(heartRateRecord.Class)));
                permissionsToGrant.Add(HealthPermission.GetReadPermission(Kotlin.Jvm.Internal.Reflection.GetOrCreateKotlinClass(distanceRecord.Class)));
                permissionsToGrant.Add(HealthPermission.GetReadPermission(Kotlin.Jvm.Internal.Reflection.GetOrCreateKotlinClass(hrvClass)));

                var grantedPermissions = await _healthConnectClient.GetGrantedPermissions();
                bool needsPermissions = true;
                if (grantedPermissions != null && grantedPermissions.Count > 0)
                {
                    var iterator = permissionsToGrant.Iterator();
                    needsPermissions = false;
                    while (iterator.HasNext)
                    {
                        var permission = iterator.Next()?.ToString();
                        if (permission != null && !grantedPermissions.Contains(permission))
                        {
                            needsPermissions = true;
                            break;
                        }
                    }
                }

                if (needsPermissions)
                {
                    var result = await PermissionHandler.Request(permissionsToGrant);
                    if (result == null || result.Count == 0)
                        return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task LoadAllHealthDataAsync()
        {
            DateTime now = DateTime.Now;
            DateTime startOfToday = now.Date;
            DateTime startOfMonth = startOfToday.AddDays(-30);

            var startToday = DateTimeToInstant(startOfToday);
            var endNow = DateTimeToInstant(now);
            var startMonth = DateTimeToInstant(startOfMonth);

            var tasks = new List<Task>
            {
                LoadSteps(startToday, endNow),
                LoadSleep(startMonth, endNow),
                LoadHeartRate(startMonth, endNow),
                LoadDistance(startToday, endNow),
                LoadHRV(startToday, endNow)
            };

            await Task.WhenAll(tasks);
        }

        private async Task LoadSteps(Instant startTime, Instant endTime)
        {
            var records = await _healthConnectClient.ReadStepsRecords(startTime, endTime);
            long totalSteps = 0;
            if (records != null)
            {
                foreach (var r in records)
                    totalSteps += r.Count;
            }
            TotalSteps = (int)totalSteps;
        }

        private async Task LoadSleep(Instant startTime, Instant endTime)
        {
            var records = await _healthConnectClient.ReadSleepRecords(startTime, endTime);
            double totalHours = 0;
            if (records != null)
            {
                foreach (var r in records)
                {
                    double hours = Java.Time.Duration.Between(r.StartTime, r.EndTime).ToMinutes() / 60.0;
                    totalHours += hours;
                }
            }
            SleepHours = totalHours;
        }

        private async Task LoadHeartRate(Instant startTime, Instant endTime)
        {
            var records = await _healthConnectClient.ReadHeartRateRecords(startTime, endTime);
            double sum = 0;
            int count = 0;
            if (records != null)
            {
                foreach (var r in records)
                {
                    if (r.Samples != null)
                    {
                        foreach (var s in r.Samples)
                        {
                            sum += s.BeatsPerMinute;
                            count++;
                        }
                    }
                }
            }
            HeartRate = count > 0 ? (int)(sum / count) : 0;
        }

        private async Task LoadDistance(Instant startTime, Instant endTime)
        {
            var records = await _healthConnectClient.ReadDistanceRecords(startTime, endTime);
            double totalMeters = 0;
            if (records != null)
            {
                foreach (var r in records)
                    totalMeters += r.Distance.Meters;
            }
            DistanceKm = totalMeters / 1000.0;
        }

        private async Task LoadHRV(Instant startTime, Instant endTime)
        {
            var records = await _healthConnectClient.ReadHeartRateVariabilityRecords(startTime, endTime);
            double totalRmssd = 0;
            int count = 0;
            if (records != null)
            {
                foreach (var r in records)
                {
                    // Aquí puedes extraer el valor RMSSD
                    // totalRmssd += r.HeartRateVariabilityMillis;
                    count++;
                }
            }
            HRV = count > 0 ? (int)(totalRmssd / count) : 0;
        }
#endif
    }
}
