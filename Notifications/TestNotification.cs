using UnityEngine;

namespace _Project.Scripts.Notifications
{
    public class TestNotification : MonoBehaviour, INotifier
    {
        [SerializeField] private Sprite _sprite;
        
        [ContextMenu("ShowInfoPanel")]
        public void ShowInfoPanel()
        {
            NotificationHelper.SendNotification(NotificationType.Info, "Test Notification",
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                "Nam congue, nulla eu tincidunt suscipit, augue felis cursus tellus, " +
                "vel egestas felis massa vel lacus. Interdum et malesuada fames ac " +
                "ante ipsum primis in faucibus. Quisque in tincidunt sem, in luctus elit. " +
                "Duis in sagittis enim, vel viverra nisi. Vestibulum luctus semper placerat. " +
                "Nam consequat gravida lectus at condimentum. Quisque sit amet ante odio. Morbi id ante eleifend," +
                " commodo nisl a, convallis justo.\n\nDuis orci libero, volutpat ut felis bibendum," +
                " scelerisque facilisis lacus. Vivamus non erat neque. " +
                "Aenean fringilla felis rhoncus mauris rhoncus, et bibendum tellus eleifend. " +
                "Nam quis sem vehicula arcu cursus volutpat ut sit amet nulla." +
                " Nullam massa odio, lobortis sit amet metus vel, fringilla consectetur ipsum." +
                " Orci varius natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. " +
                "Vivamus laoreet nibh orci, id blandit nisi mattis ut. Phasellus nisi felis, " +
                "dapibus sed lectus ut, tempus dapibus nibh. Cras aliquet lacus eget libero vestibulum," +
                " sit amet condimentum nulla consequat. Donec ac ex id elit suscipit ullamcorper. " +
                "Integer non tortor et metus auctor viverra sit amet a leo. Praesent ante mi, viverra id fringilla eu, " +
                "cursus sit amet est. Etiam massa nibh, porta non metus et, porttitor auctor sapien.",
                this, NotifyCallType.Open);    
        }
        
        [ContextMenu("ShowFieldPanel")]
        public void ShowFieldPanel()
        {
            NotificationHelper.SendNotification(NotificationType.SingleRequiredField, "Test Notification",
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                "Nam congue, nulla eu tincidunt suscipit, augue felis cursus tellus, " +
                "vel egestas felis massa vel lacus. Interdum et malesuada fames ac " +
                "ante ipsum primis in faucibus. Quisque in tincidunt sem, in luctus elit. " +
                "Duis in sagittis enim, vel viverra nisi. Vestibulum luctus semper placerat. " +
                "Nam consequat gravida lectus at condimentum. Quisque sit amet ante odio. Morbi id ante eleifend," +
                " commodo nisl a, convallis justo.\n\nDuis orci libero, volutpat ut felis bibendum," +
                " scelerisque facilisis lacus. Vivamus non erat neque. " +
                "Aenean fringilla felis rhoncus mauris rhoncus, et bibendum tellus eleifend. " +
                "Nam quis sem vehicula arcu cursus volutpat ut sit amet nulla." +
                " Nullam massa odio, lobortis sit amet metus vel, fringilla consectetur ipsum." +
                " Orci varius natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. " +
                "Vivamus laoreet nibh orci, id blandit nisi mattis ut. Phasellus nisi felis, " +
                "dapibus sed lectus ut, tempus dapibus nibh. Cras aliquet lacus eget libero vestibulum," +
                " sit amet condimentum nulla consequat. Donec ac ex id elit suscipit ullamcorper. " +
                "Integer non tortor et metus auctor viverra sit amet a leo. Praesent ante mi, viverra id fringilla eu, " +
                "cursus sit amet est. Etiam massa nibh, porta non metus et, porttitor auctor sapien.",
                this, NotifyCallType.Open);
        }
        
        [ContextMenu("ShowProgressNotification")]
        public void ShowProgressNotification()
        {
            NotificationHelper.SendNotification(NotificationType.Progress, "Test Notification", 
                "Testing Notification", this, NotifyCallType.Open);
        }
        
        [ContextMenu("HideProgressNotification")]
        public void HideProgressNotification()
        {
            NotificationHelper.SendNotification(NotificationType.Progress, "Test Notification", 
                "Notification Tested", this, NotifyCallType.Close);
        }
        
        [ContextMenu("ShowLoadingScreen")]
        public void ShowLoadingScreen()
        {
            NotificationHelper.SendNotification(NotificationType.LoadingScreen, "Test Notification",
                "Testing Notification", this, NotifyCallType.Open);
        }
        [ContextMenu("HideLoadingScreenNotification")]
        public void HideLoadingScreenNotification()
        {
            NotificationHelper.SendNotification(NotificationType.LoadingScreen, "Test Notification",
                "Notification Tested", this, NotifyCallType.Close);
        }
        
        [ContextMenu("SimpleSlideNotification")]
        public void SimpleSlideNotification()
        {
            NotificationHelper.SendNotification(NotificationType.SlidingText, "Test Notification",
                "This is a Test Notification", this, NotifyCallType.Open);
        }
        
        [ContextMenu("ResponseSlideNotification")]
        public void ResponseSlideNotification()
        {
            NotificationHelper.SendNotification(NotificationType.SlidingResponse, "Test Notification",
                "This is a Test Notification", this, NotifyCallType.Open);
        }
        
        [ContextMenu("ShowGraphicPanel")]
        public void ShowGraphicPanel()
        {
            NotificationHelper.SendNotification(NotificationType.GraphicWindow, "Test Notification",
                "This is a Test Notification", this, NotifyCallType.Open,
                GraphicPanelType.Custom, _sprite);
        }
        

        public void Notify(string notifyData = null, bool accepted = false, bool rejected = false)
        {
            if(!string.IsNullOrEmpty(notifyData))
                Debug.Log(notifyData);
            
            if (accepted)
            {
                Debug.Log("Accepted");
            }

            if (rejected)
            {
                Debug.Log("Declined");
            }
        }
    }
}
