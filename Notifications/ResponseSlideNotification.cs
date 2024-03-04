using System;
using System.Collections;
using _Project.Scripts.HelperClasses;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Notifications
{
    public class ResponseSlideNotification : SlidingNotification, INotification
    {
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private float _showTime;
        [SerializeField] private float _hideTime;
        [SerializeField] private Button _accept;
        [SerializeField] private Button _decline;
        
        public NotificationType NotificationType => _type;
        public bool NotifyOnClose { private set; get; }
        
        protected void Awake() => Notification = this;

        protected override IEnumerator Start()
        {
            yield return base.Start();
            gameObject.SetActive(false);
        }

        public void ShowNotification(NotificationData nData)
        {
            switch (nData.NotifyCallType)
            {
                case NotifyCallType.Open:
                    gameObject.SetActive(true);
                    _title.SetText($"{nData.Context}");
                    _description.SetText($"{nData.Text}");
                    
                    if (ShowSlideNotificationCo != null) StopCoroutine(ShowSlideNotificationCo);
                    ShowSlideNotificationCo = StartCoroutine(ShowNotification());
                    
                    _accept.onClick.RemoveAllListeners();
                    _accept.onClick.AddListener(() =>
                    {
                        if (HideSlideNotificationCo != null) StopCoroutine(HideSlideNotificationCo);
                        HideSlideNotificationCo = StartCoroutine(HideNotification());
                        
                        NotificationHelper.SendNotification(NotificationType.SlidingResponse, nData.Context,nData.Text,
                            nData.Notifier, NotifyCallType.Close);
                        nData.Notifier.Notify("", true, false);
                    });
                    
                    _decline.onClick.RemoveAllListeners();
                    _decline.onClick.AddListener(() =>
                    {
                        if (HideSlideNotificationCo != null) StopCoroutine(HideSlideNotificationCo);
                        HideSlideNotificationCo = StartCoroutine(HideNotification());
                        
                        NotificationHelper.SendNotification(NotificationType.SlidingResponse, nData.Context,nData.Text,
                            nData.Notifier, NotifyCallType.Close);
                        nData.Notifier.Notify("", false, true);
                    });
                    NotifyOnClose = false;
                    break;
                case NotifyCallType.Close:
                    NotifyOnClose = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public override IEnumerator ShowNotification()
        {
            yield return HideNotification();
            
            RectTransform.SetPivot(GetPivot(_slidingSide));
            RectTransform.SetAnchor(GetAnchorPresets(_slidingSide), GetOffset(_slidingSide));
            yield return SlidingNotificationTo(0, _showTime);
        }

        public override IEnumerator HideNotification()
        {
            yield return SlidingNotificationTo(GetResetPosition(), _hideTime);
        }
    }
}
