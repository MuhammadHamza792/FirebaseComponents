using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Notifications.Android;
using UnityEngine;
using UnityEngine.Android;
using Object = System.Object;

namespace _Project.Scripts.ThirdPartyNotifications
{
    public class AndroidNotifications : MonoBehaviour
    {
        private Dictionary<string, AndroidNotificationChannelGroup> _notificationGroups;
        private Dictionary<string, AndroidNotificationChannel> _notificationChannels;
        
        private int _canAskForNotificationPermission;
        private int _notificationPermissionGranted;
        private int _canShowNotifications;

        public int CanShowNotifications
        {
            set
            {
                _canShowNotifications = value;
                PlayerPrefs.SetInt("ShowNotifications", _canShowNotifications);
            }
            get => _canShowNotifications;
        }

        private Coroutine _notifyPermissionCo;

        private void Awake()
        {
            _notificationGroups = new Dictionary<string, AndroidNotificationChannelGroup>();
            _notificationChannels = new Dictionary<string, AndroidNotificationChannel>();
        }

        private void Start()
        {
            _canShowNotifications = PlayerPrefs.GetInt("ShowNotifications", 1);
            _notificationPermissionGranted = PlayerPrefs.GetInt("NotificationPermissionGranted", 0);
            _canAskForNotificationPermission = PlayerPrefs.GetInt("AskForNotificationPermission", 1);
        }

        private bool _requestingNotificationsPermission;
        public void RequestForNotifications()
        {
            if(_requestingNotificationsPermission) return;
            _requestingNotificationsPermission = true;

            if (_notificationPermissionGranted != 0 || _canAskForNotificationPermission != 1)
            {
                _requestingNotificationsPermission = false;
                Debug.LogWarning("Can't show notifications, user hasn't approved.");
                return;
            }
            
            if (_notifyPermissionCo != null) StopCoroutine(_notifyPermissionCo);
            _notifyPermissionCo = StartCoroutine(RequestNotificationPermission());
        }
        private IEnumerator RequestNotificationPermission()
        {
            var request = new PermissionRequest();
            while (request.Status == PermissionStatus.RequestPending) yield return null;
            switch (request.Status)
            {
                case PermissionStatus.NotRequested:
                    PlayerPrefs.SetInt("NotificationPermissionGranted", 0);
                    PlayerPrefs.SetInt("AskForNotificationPermission", 1);
                    break;
                case PermissionStatus.Allowed:
                    PlayerPrefs.SetInt("NotificationPermissionGranted", 1);
                    PlayerPrefs.SetInt("AskForNotificationPermission", 0);
                    break;
                case PermissionStatus.Denied:
                    PlayerPrefs.SetInt("NotificationPermissionGranted", 0);
                    PlayerPrefs.SetInt("AskForNotificationPermission", 1);
                    break;
                case PermissionStatus.DeniedDontAskAgain:
                    PlayerPrefs.SetInt("NotificationPermissionGranted", 0);
                    PlayerPrefs.SetInt("AskForNotificationPermission", 0);
                    break;
                case PermissionStatus.RequestPending:
                    PlayerPrefs.SetInt("NotificationPermissionGranted", 0);
                    PlayerPrefs.SetInt("AskForNotificationPermission", 1);
                    break;
                case PermissionStatus.NotificationsBlockedForApp:
                    PlayerPrefs.SetInt("NotificationPermissionGranted", 0);
                    PlayerPrefs.SetInt("AskForNotificationPermission", 1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _requestingNotificationsPermission = false;
        }
        
        public void CreateAndRegisterGroup(AndroidNotificationChannelGroup groupData)
        {
            if (_notificationGroups.ContainsKey(groupData.Id))
            {
                Debug.LogError("Can't create the group. Group already exists!");
                return;
            }
            
            _notificationGroups.Add(groupData.Id, groupData);
            RegisterNotificationChannelGroup(groupData.Id);
        }
        public void CreateGroup( AndroidNotificationChannelGroup groupData)
        {
            if (_notificationGroups.ContainsKey(groupData.Id))
            {
                Debug.LogError("Can't create the group. Group already exists!");
                return;
            }
            _notificationGroups.Add(groupData.Id, groupData);
        }
        public void RegisterNotificationChannelGroup(string groupId)
        {
            if (!_notificationGroups.ContainsKey(groupId))
            {
                Debug.LogError("Can't register. Group doesn't exist!");
                return;
            }

            AndroidNotificationCenter.RegisterNotificationChannelGroup(_notificationGroups[groupId]);
        }
        public void DeleteNotificationChannelGroup(string groupId)
        {
            if (!_notificationGroups.ContainsKey(groupId))
            {
                Debug.LogError("Can't delete group. This group doesn't exist!");
                return;
            }
            
            AndroidNotificationCenter.DeleteNotificationChannelGroup(groupId);
        }
        
        public void CreateAndRegisterCustomNotificationChannel(AndroidNotificationChannel channel)
        {
            if (!_notificationGroups.ContainsKey(channel.Group))
            {
                Debug.LogError("Can't create channel. Group doesn't exist!");
                return;
            }
            
            if (_notificationChannels.ContainsKey(channel.Id))
            {
                Debug.LogError("Can't create the channel. Channel already exists!");
                return;
            }
            
            _notificationChannels.Add(channel.Id, channel);
            AndroidNotificationCenter.RegisterNotificationChannel(channel);
        }
        public void CreateCustomNotificationChannel(AndroidNotificationChannel channel)
        {
            if (!_notificationGroups.ContainsKey(channel.Group))
            {
                Debug.LogError("Can't create channel. Group doesn't exist!");
                return;
            }
            
            if (_notificationChannels.ContainsKey(channel.Id))
            {
                Debug.LogError("Can't create the channel. Channel already exists!");
                return;
            }
            
            _notificationChannels.Add(channel.Id, channel);
        }
        public void RegisterNotificationChannel(string channelId)
        {
            if (!_notificationChannels.ContainsKey(channelId))
            {
                Debug.LogError("Can't register the channel. Channel doesn't exists!");
                return;
            }
            
            AndroidNotificationCenter.RegisterNotificationChannel(_notificationChannels[channelId]);
        }
        public AndroidNotificationChannel? GetNotificationChannel(string channelId, string groupId)
        {
            if (!_notificationGroups.ContainsKey(groupId))
            {
                Debug.LogError("Can't fetch channel. Group doesn't exist!");
                return null;
            }
            
            if (!_notificationChannels.ContainsKey(channelId))
            {
                Debug.LogError("Can't fetch channel. Channel doesn't exists!");
                return null;
            }

            return AndroidNotificationCenter.GetNotificationChannel(channelId);
        }
        public AndroidNotificationChannel[] GetNotificationChannels() => AndroidNotificationCenter.GetNotificationChannels();
        public void OpenNotificationsSettings(string channelId, string groupId)
        {
            if (!_notificationGroups.ContainsKey(groupId))
            {
                Debug.LogError("Can't open channels Settings. Group doesn't exist!");
                return;
            }
            
            if (!_notificationChannels.ContainsKey(channelId))
            {
                Debug.LogError("Can't open channels Settings. Channel doesn't exists!");
                return;
            }
            
            AndroidNotificationCenter.OpenNotificationSettings(channelId);
        }
        public void DeleteNotificationChannel(string channelId)
        {
            if (!_notificationChannels.ContainsKey(channelId))
            {
                Debug.LogError("Can't delete channel. This channel doesn't exist!");
                return;
            }
            
            AndroidNotificationCenter.DeleteNotificationChannel(channelId);
        }
        
        public void CancelNotification(int notificationId)
        {
            if (CheckIfNotificationIdDoesntExists(notificationId))
            {
                Debug.LogError("Notification Id does not exist!");
                return;
            }
            
            AndroidNotificationCenter.CancelNotification(notificationId);
        }
        public void CancelAllNotifications() => AndroidNotificationCenter.CancelAllNotifications();
        
        public void UpdateNotificationScheduling(int notificationId, AndroidNotification androidNotification, string channelId)
        {
            if (!_notificationChannels.ContainsKey(channelId))
            {
                Debug.LogError("Can't update notification scheduling. This channel doesn't");
                return;
            }
            
            if (CheckIfNotificationIdDoesntExists(notificationId))
            {
                Debug.LogError("Notification Id does not exist!");
                return;
            }
            
            AndroidNotificationCenter.UpdateScheduledNotification(notificationId, androidNotification, channelId);
        }
        public void CancelScheduledNotification(int notificationId)
        {
            if (CheckIfNotificationIdDoesntExists(notificationId))
            {
                Debug.LogError("Notification Id does not exist!");
                return;
            }
            AndroidNotificationCenter.CancelScheduledNotification(notificationId);
        }
        public void CancelAllScheduledNotifications() => AndroidNotificationCenter.CancelAllScheduledNotifications();
        
        public void CancelDisplayedNotification(int notificationId)
        {
            if (CheckIfNotificationIdDoesntExists(notificationId))
            {
                Debug.LogError("Notification Id does not exist!");
                return;
            }
            
            AndroidNotificationCenter.CancelDisplayedNotification(notificationId);
        }
        public void CancelAllDisplayedNotification() => AndroidNotificationCenter.CancelAllDisplayedNotifications();

        public bool CheckIfNotificationIdDoesntExists(int notificationId) => AndroidNotificationCenter.CheckScheduledNotificationStatus(notificationId) == NotificationStatus.Unknown;
        public bool IsNotificationScheduled(int notificationId) => AndroidNotificationCenter.CheckScheduledNotificationStatus(notificationId) == NotificationStatus.Scheduled;
        public bool IsNotificationDelivered(int notificationId) => AndroidNotificationCenter.CheckScheduledNotificationStatus(notificationId) == NotificationStatus.Delivered;
        public bool CheckIfNotificationIsntAvailable(int notificationId) => AndroidNotificationCenter.CheckScheduledNotificationStatus(notificationId) == NotificationStatus.Unavailable;
        public NotificationStatus CheckScheduledNotificationStatus(int notificationId) => AndroidNotificationCenter.CheckScheduledNotificationStatus(notificationId);
        
        public void GetLastNotificationIntent() => AndroidNotificationCenter.GetLastNotificationIntent();
        public int? SendNotification(string channelId, AndroidNotification androidNotification, int? customId)
        {
            if (_notificationPermissionGranted == 0)
            {
                RequestForNotifications();
                return null;
            }

            if (_canShowNotifications == 0)
            {
                Debug.LogWarning("Can't show notifications, user has disabled notifications.");
                return null;
            }
            
            if (!_notificationChannels.ContainsKey(channelId))
            {
                Debug.LogError("Can't show notifications, This channel doesn't exist!");
                return null;
            }

            if (customId == null)
            {
                return AndroidNotificationCenter.SendNotification(androidNotification, channelId);
            }
            else
            {
                AndroidNotificationCenter.SendNotificationWithExplicitID(androidNotification, channelId, customId.Value);
                return customId;
            }
        }
    }
}