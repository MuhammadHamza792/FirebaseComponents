using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Notifications
{
    public class GraphicPanel : LobbyNotification, INotification
    {
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private Button _continue;
        [SerializeField] private Button _cancel;

        [SerializeField] private List<GraphicsForPanel> _graphicsForPanel;
        
        public bool NotifyOnClose { private set;  get;}
        
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
                    gameObject.SetActive(true);
                    NotifyOnClose = false;
                    _title.SetText($"{nData.Context}");
                    _description.SetText($"{nData.Text}");
                    
                    _continue.onClick.RemoveAllListeners();
                    _continue.onClick.AddListener(() =>
                    {
                        NotificationHelper.SendNotification(NotificationType.GraphicWindow, nData.Context,nData.Text,
                            nData.Notifier, NotifyCallType.Close);
                        nData.Notifier.Notify("", true, false);
                    });
                    
                    _cancel.onClick.RemoveAllListeners();
                    _cancel.onClick.AddListener(() =>
                    {
                        NotificationHelper.SendNotification(NotificationType.GraphicWindow, nData.Context,nData.Text,
                            nData.Notifier, NotifyCallType.Close);
                        nData.Notifier.Notify("", false, true);
                    });
                    
                    SetPanelGraphic(nData.GraphicType, nData.Graphic);
                    break;
                case NotifyCallType.Close:
                    gameObject.SetActive(false);
                    NotifyOnClose = true;
                    break;
            }
        }

        private void SetPanelGraphic(GraphicPanelType graphicType, Sprite graphic)
        {
            switch (graphicType)
            {
                case GraphicPanelType.None:
                    _image.enabled = false;
                    break;
                case GraphicPanelType.Warning:
                    SetImage(GetSprite(graphicType));
                    break;
                case GraphicPanelType.Critical:
                    SetImage(GetSprite(graphicType));
                    break;
                case GraphicPanelType.Custom:
                    SetImage(graphic);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(graphicType), graphicType, null);
            }
        }

        private void SetImage(Sprite graphic)
        {
            _image.enabled = true;
            _image.sprite = graphic;
            _image.preserveAspect = true;
        }

        private Sprite GetSprite(GraphicPanelType graphicType)
        {
            return _graphicsForPanel.
                FirstOrDefault(pGraphic => pGraphic.Name == graphicType.ToString())?.Sprite;
        }
    }

    public enum GraphicPanelType
    {
        None,
        Warning,
        Critical,
        Custom
    }

    [Serializable]
    public class GraphicsForPanel
    {
        public string Name;
        public Sprite Sprite;
    }
}
