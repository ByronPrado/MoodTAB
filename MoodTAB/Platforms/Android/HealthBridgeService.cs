using Android.Content;
//using Com.Example.Healthbridge; 
using MoodTAB.Services;
using Kotlin.Coroutines;

namespace MoodTAB.Platforms.Android
{
    public class HealthBridgeService
    {
        public static Task<long> GetTodayStepsAsync()
        {
            var tcs = new TaskCompletionSource<long>();
            var context = Application.BindingContextProperty;

            // Ejecutamos la tarea asíncrona usando Task.Run
            Task.Run(async () =>
            {
                try
                {
                    //long steps = await HealthConnectBridge.GetTodaySteps(context);
                    long steps = 10000;
                    tcs.SetResult(steps);
                }
                catch (System.Exception ex)
                {
                    tcs.SetException(ex);
                }
            });            return tcs.Task;
        }
    }
}
