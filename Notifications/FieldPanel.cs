using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Notifications
{
    public class FieldPanel : LobbyNotification,INotification
    {
        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TMP_InputField _field;
        [SerializeField] private Button _continue; 
        [SerializeField] private Button _closeButton;
        
        public bool NotifyOnClose { private set;  get;}
        
        public NotificationType NotificationType => _type;
        
        private void Awake()
        {
            Notification = this;
            gameObject.SetActive(false);
        }
        
        public void ShowNotification(NotificationData nData)
        {
            _closeButton.onClick.RemoveAllListeners();
            _closeButton.onClick.AddListener(() =>
            {
                NotificationHelper.SendNotification(NotificationType.SingleRequiredField, nData.Context,nData.Text,
                    nData.Notifier, NotifyCallType.Close);
            });
            
            switch (nData.NotifyCallType)
            {
                case NotifyCallType.Open:
                    _field.text = string.Empty;
                    _description.SetText($"{nData.Text}");
                    _title.SetText($"{nData.Context}");
                    gameObject.SetActive(true);
                    _continue.onClick.RemoveAllListeners();
                    _continue.onClick.AddListener(() =>
                    {
                        NotificationHelper.SendNotification(NotificationType.SingleRequiredField, nData.Context,nData.Text,
                            nData.Notifier, NotifyCallType.Close);
                        nData.Notifier.Notify(_field.text);
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