namespace MoodTAB.Services
{
    public interface INotificationManagerService
    {
        event EventHandler NotificationReceived;
        void SendNotification(string title, string message, DateTime? notifyTime = null, int id = 0);
        void ReceiveNotification(string title, string message);
        void DeleteNotification(int id);
    }
}