using UnityEngine;

namespace _Project.Scripts.Notifications
{
    public static class NotificationHelper
    {
        public static void SendNotification(NotificationType type, string context, string msg, 
            INotifier notifier, NotifyCallType callType, 
            GraphicPanelType graphicType = GraphicPanelType.None, Sprite graphic = null)
        {
            if(NotificationHandler.Instance == null) return;
            
            var nData = new NotificationData
            {
                NotifyType = type,
                Context = context,
                Text = msg,
                Notifier = notifier,
                NotifyCallType = callType,
                GraphicType = graphicType,
                Graphic = graphic
            };
            
            NotificationHandler.Instance.HandleNotificationPanel(nData);
        }
        
        public static string GetButtonText(string textToSet, string defaultValue = null) => string.IsNullOrEmpty(textToSet) || string.IsNullOrWhiteSpace(textToSet) ? defaultValue : textToSet;
    }
}