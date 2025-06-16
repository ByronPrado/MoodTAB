#if ANDROID
using Android.App;
using Android.App.Usage;
using Android.Content;
using Android.OS;
using Java.Util;
using System;
using System.Collections.Generic;

public class UsageStatsHelper
{
    public static Dictionary<string, long> GetAppUsageStats()
    {
        var context = Android.App.Application.Context;
        var usageStatsManager = (UsageStatsManager)context.GetSystemService(Context.UsageStatsService);

        var endTime = Java.Lang.JavaSystem.CurrentTimeMillis();
        var startTime = endTime - 1000 * 60 * 60 * 24; // Últimas 24 horas

        var usageStatsList = usageStatsManager.QueryUsageStats(
            UsageStatsInterval.Daily,
            startTime,
            endTime);

        var appUsage = new Dictionary<string, long>();
        foreach (var stat in usageStatsList)
        {
            if (stat.TotalTimeInForeground > 0)
            {
                appUsage[stat.PackageName] = stat.TotalTimeInForeground;
            }
        }

        return appUsage;
    }

        public static void OpenUsageAccessSettings()
    {
        var intent = new Intent(Android.Provider.Settings.ActionUsageAccessSettings);
        intent.SetFlags(ActivityFlags.NewTask);
        Android.App.Application.Context.StartActivity(intent);
    }
}
#endif
