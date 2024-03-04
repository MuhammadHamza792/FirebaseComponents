namespace _Project.Scripts.Notifications
{
    public interface INotification
    {
        public NotificationType NotificationType { get; }
        public bool NotifyOnClose { get; }
        public void ShowNotification(NotificationData nData);
    }
}