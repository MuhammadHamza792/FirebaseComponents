using UnityEngine;

namespace _Project.Scripts.Notifications
{
    public class LobbyNotification : MonoBehaviour
    {
        [SerializeField] protected NotificationType _type;
        
        protected NotificationHandler Handler;
        
        public INotification Notification { protected set; get; }
        
        public void SetHandler(NotificationHandler notificationHandler)
        {
            Handler = notificationHandler;
        }
    }
}
