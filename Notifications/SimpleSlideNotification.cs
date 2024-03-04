using System;
using System.Collections;
using _Project.Scripts.HelperClasses;
using TMPro;
using UnityEngine;

namespace _Project.Scripts.Notifications
{
    public class SimpleSlideNotification : SlidingNotification,INotification
    {
        [SerializeField] private float _showTime;
        [SerializeField] private float _hideTime;
        [SerializeField] private float _stayTime;
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _description;
        
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
            NData = nData;
            
            switch (nData.NotifyCallType)
            {
                case NotifyCallType.Open:
                    gameObject.SetActive(true);
                    _title.SetText($"{nData.Context}");
                    _description.SetText($"{nData.Text}");
                    SlideNotification();
                    NotifyOnClose = false;
                    break;
                case NotifyCallType.Close:  
                    NotifyOnClose = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [ContextMenu("SlideNotification")]
        public void SlideNotification()
        {
            if (ShowSlideNotificationCo != null) StopCoroutine(ShowSlideNotificationCo);
            ShowSlideNotificationCo = StartCoroutine(ShowNotification());
        }

        public override IEnumerator ShowNotification()
        {
            yield return HideNotification();
            
            RectTransform.SetPivot(GetPivot(_slidingSide));
            RectTransform.SetAnchor(GetAnchorPresets(_slidingSide), GetOffset(_slidingSide));
            yield return SlidingNotificationTo(0, _showTime);
            
            yield return EnumeratorHelper.AddDelay(_stayTime);

            yield return HideNotification();
            
            CloseNotification();
        }

        public override IEnumerator HideNotification()
        {
            yield return SlidingNotificationTo(GetResetPosition(), _hideTime);
        }

        private void CloseNotification() =>
            NotificationHelper.SendNotification(NotificationType.SlidingText, NData.Context,NData.Text,
                NData.Notifier, NotifyCallType.Close);
        
    }
}

