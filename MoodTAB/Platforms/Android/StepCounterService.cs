#if ANDROID
using Android.Content;
using Android.Hardware;
using MoodTAB.Services;
using Android.Runtime;

namespace MoodTAB.Platforms.Android
{
    public class StepCounterService : Java.Lang.Object, ISensorEventListener, IStepCounterService
    {
        private SensorManager? sensorManager;
        private Sensor? stepSensor;

        public long TotalSteps { get; private set; } = 0;

        public void Start()
        {
            sensorManager = (SensorManager?)global::Android.App.Application.Context.GetSystemService(Context.SensorService);
            stepSensor = sensorManager?.GetDefaultSensor(SensorType.StepCounter);

            if (stepSensor != null)
            {
                sensorManager.RegisterListener(this, stepSensor, SensorDelay.Ui);
            }
        }

        public void Stop()
        {
            if (sensorManager != null)
            {
                sensorManager.UnregisterListener(this);
            }
        }

        public void OnSensorChanged(SensorEvent? e)
        {
            if (e != null && e.Sensor?.Type == SensorType.StepCounter)
            {
                TotalSteps = (long)e.Values[0]; // Total desde el último reinicio del dispositivo
            }
        }

        public void OnAccuracyChanged(Sensor? sensor, [GeneratedEnum] SensorStatus accuracy)
        {
            // Puedes manejar esto si necesitas detectar cambios de precisión
        }
    }
}
#endif
