using System;
using TMPro;
using UnityEngine;

namespace _Project.Scripts.Notifications
{
    public class LoadingPanel : LobbyNotification, INotification
    {
        [SerializeField] private TextMeshProUGUI _progress;

        public NotificationType NotificationType => _type;
        
        public bool NotifyOnClose { private set; get; }
        
        private void Awake()
        {
            Notification = this;
            gameObject.SetActive(false);
        }
        
        public void ShowNotification(NotificationData nData)
        {
            switch (nData.NotifyCallType)
            {
                case NotifyCallType.Open:
                    _progress.SetText($"{nData.Text}");
                    gameObject.SetActive(true);
                    NotifyOnClose = true;
                    break;
                case NotifyCallType.Close:
                    _progress.SetText($"{nData.Text}");
                    Invoke(nameof(DisableObject),.75f);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DisableObject()
        {
            gameObject.SetActive(false);
            NotifyOnClose = false;
        }
    }
}
