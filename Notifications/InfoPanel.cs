using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Notifications
{
    public class InfoPanel : LobbyNotification,INotification
    {
        [SerializeField] private Button _continue;
        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private TextMeshProUGUI _title;
        
        public bool NotifyOnClose { private set;  get; }

        public NotificationType NotificationType => _type;
        
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
                    _description.SetText($"{nData.Text}");
                    _title.SetText($"{nData.Context}");
                    gameObject.SetActive(true);
                    _continue.onClick.RemoveAllListeners();
                    _continue.onClick.AddListener(() =>
                    {
                        NotificationHelper.SendNotification(NotificationType.Info, nData.Context,nData.Text,
                            nData.Notifier, NotifyCallType.Close);
                        nData.Notifier.Notify();
                    });
                    NotifyOnClose = false;
                    break;
                case NotifyCallType.Close:
                    gameObject.SetActive(false);
                    NotifyOnClose = true;
                    break;
            }
        }
    }
}