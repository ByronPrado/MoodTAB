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
    public static Dictionary<string, long>? GetAppUsageStats()
    {
        var context = Android.App.Application.Context;
        var usageStatsManager = context.GetSystemService(Context.UsageStatsService) as UsageStatsManager;
        var endTime = Java.Lang.JavaSystem.CurrentTimeMillis();
        var startTime = endTime - 1000 * 60 * 60 * 24; // Últimas 24 horas
        var appUsage = new Dictionary<string, long>(); //si devolvemos este vacio significa error.
        if (usageStatsManager != null)
        {
            var usageStatsList = usageStatsManager.QueryUsageStats(
            UsageStatsInterval.Daily,
            startTime,
            endTime);
            if (usageStatsList != null)
            {
                foreach (var stat in usageStatsList)
                {
                    if (stat.TotalTimeInForeground > 0 && !string.IsNullOrEmpty(stat.PackageName))
                    {
                        appUsage[stat.PackageName] = stat.TotalTimeInForeground;
                    }
                }
                return appUsage;
            }
            else return null;
        }
        else return null;
    }

    public static void OpenUsageAccessSettings()
    {
        var intent = new Intent(Android.Provider.Settings.ActionUsageAccessSettings);
        intent.SetFlags(ActivityFlags.NewTask);
        Android.App.Application.Context.StartActivity(intent);
    }
    public static bool TienePermisoDeUso()
    {
        var context = Android.App.Application.Context;
        AppOpsManager? appOps = context.GetSystemService(Context.AppOpsService) as AppOpsManager;
        int mode;
        if (appOps != null && context.PackageName != null)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Q && OperatingSystem.IsAndroidVersionAtLeast(29)) // Android 10 (API 29) and above
            {
                mode = (int)appOps.UnsafeCheckOpNoThrow("android:get_usage_stats", Process.MyUid(), context.PackageName);
            }
            else
            {
                mode = (int)appOps.CheckOpNoThrow("android:get_usage_stats", Process.MyUid(), context.PackageName);
            }
            return mode == (int)AppOpsManagerMode.Allowed;
        }
        else return false;        
    }
}
#endif