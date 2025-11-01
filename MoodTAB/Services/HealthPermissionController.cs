#if ANDROID
using AndroidX.Activity.Result.Contract;
using AndroidX.Health.Connect.Client.Contracts;
#endif
namespace MoodTAB.Services
{
    public static class HealthPermissionController
    {
        #if ANDROID
        public static ActivityResultContract CreateRequestPermissionResultContract()
        {
            return new HealthPermissionsRequestContract();
        }
        #endif
    }
}
