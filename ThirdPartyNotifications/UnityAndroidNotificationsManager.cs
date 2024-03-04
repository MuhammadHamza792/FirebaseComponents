using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Notifications.Android;
using UnityEngine;
using UnityEngine.Android;

namespace _Project.Scripts.ThirdPartyNotifications
{
    public class UnityAndroidNotificationsManager : MonoBehaviour
    {
        [SerializeField] private List<AndroidNotificationGroupData> _notificationData;

        private AndroidNotifications _androidNotifications;

        public event Action<AndroidNotificationIntentData> OnApplicationOpenedWithNotification; 
        public event Action<AndroidNotificationIntentData> OnIndentReceivedWithNotificationWhileGameIsRunning; 
        
        private void Awake() => _androidNotifications = GetComponent<AndroidNotifications>();

        private void Start()
        {
            AndroidNotificationCenter.OnNotificationReceived += ReceivedNotificationHandler;
            GetLastNotification();
        }

        private void OnDisable() => AndroidNotificationCenter.OnNotificationReceived -= ReceivedNotificationHandler;

        private void ReceivedNotificationHandler(AndroidNotificationIntentData data)
        {
            var msg = "Notification received : " + data.Id + "\n";
            msg += "\n Notification received: ";
            msg += "\n .Title: " + data.Notification.Title;
            msg += "\n .Body: " + data.Notification.Text;
            msg += "\n .Channel: " + data.Channel;
            Debug.Log(msg);
            OnIndentReceivedWithNotificationWhileGameIsRunning?.Invoke(data);
        }

        private void GetLastNotification()
        {
            var notificationIntentData = AndroidNotificationCenter.GetLastNotificationIntent();
            if (notificationIntentData == null) return;
            OnApplicationOpenedWithNotification?.Invoke(notificationIntentData);
        }

        public void CreateCustomNewNotification(AndroidNotificationChannelGroup groupData,
            AndroidNotificationChannel channelData, int notificationId ,AndroidNotification notificationData)
        {
            _androidNotifications.CreateAndRegisterGroup(groupData);
            _androidNotifications.CreateAndRegisterCustomNotificationChannel(channelData);
            
            var notificationStatus = _androidNotifications.CheckScheduledNotificationStatus(notificationId);

            switch (notificationStatus)
            {
                case NotificationStatus.Scheduled:
                    // Replace the scheduled notification with a new notification.
                    _androidNotifications.UpdateNotificationScheduling(notificationId, notificationData, channelData.Id);
                    break;
                case NotificationStatus.Delivered:
                    // Remove the previously shown notification from the status bar.
                    _androidNotifications.CancelNotification(notificationId);
                    break;
                case NotificationStatus.Unknown:
                    _androidNotifications.SendNotification(channelData.Id, notificationData, notificationId);
                    break;
            }
        }
        
        public void ScheduleAllNotifications()
        {
            for (int i = 0; i < _notificationData.Count; i++)
            {
                var notificationData = _notificationData[i];
                var groupData = new AndroidNotificationChannelGroup()
                {
                    Id = notificationData.GroupData.Id,
                    Name = notificationData.GroupData.Name,
                    Description = notificationData.GroupData.Description
                };
                _androidNotifications.CreateAndRegisterGroup(groupData);
                for (int j = 0; j < notificationData.Channels.Count; j++)
                {
                    var notificationChannel = notificationData.Channels[j];
                    var channel = new AndroidNotificationChannel()
                    {
                        Id = notificationChannel.ChannelData.Id,
                        Name = notificationChannel.ChannelData.Name,
                        Description = notificationChannel.ChannelData.Description,
                        CanBypassDnd = notificationChannel.ChannelData.CanBypassDnd,
                        CanShowBadge = notificationChannel.ChannelData.CanShowBadge,
                        EnableLights = notificationChannel.ChannelData.EnableLights,
                        EnableVibration = notificationChannel.ChannelData.EnableVibration,
                        Group = notificationData.GroupData.Id,
                        Importance = notificationChannel.ChannelData.Importance,
                        LockScreenVisibility = notificationChannel.ChannelData.LockScreenVisibility
                    };
                    
                    _androidNotifications.CreateAndRegisterCustomNotificationChannel(channel);

                    for (int k = 0; k < notificationChannel.NotificationsData.Count; k++)
                    {
                        var notification = notificationChannel.NotificationsData[k];
                        var notifyData = new AndroidNotification()
                        {
                            Title = notification.NotificationTitle,
                            Text = notification.NotificationText,
                            Color = notification.HasColor ? notification.Color : null,
                            FireTime = DateTime.Now.AddYears(notification.FireTime.Years).AddMonths(notification.FireTime.Months)
                                .AddDays(notification.FireTime.Days).AddHours(notification.FireTime.Hours).AddMinutes(notification.FireTime.Minutes)
                                .AddSeconds(notification.FireTime.Seconds).AddMilliseconds(notification.FireTime.Milliseconds),
                            RepeatInterval = notification.HasRepeatInterval ? notification.RepeatInterval : null,
                            Group = notificationChannel.Group,
                            GroupSummary = notificationChannel.GroupSummary,
                            GroupAlertBehaviour = notificationChannel.GroupAlertBehaviour,
                            IntentData = notification.IntentData,
                            UsesStopwatch = notification.UseStopWatch,
                            SortKey = notification.SortKey,
                            ShowTimestamp = notification.ShowTimestamp,
                            ShowInForeground = notification.ShowInForeground,
                            ShouldAutoCancel = notification.ShouldAutoCancel
                        };

                        if (notification.UseCustomTimestamp)
                        {
                            notifyData.CustomTimestamp = new DateTime(notification.CustomTimestamp.Years, 
                                notification.CustomTimestamp.Months, notification.CustomTimestamp.Days, 
                                notification.CustomTimestamp.Hours, notification.CustomTimestamp.Minutes,
                                notification.CustomTimestamp.Seconds, notification.CustomTimestamp.Milliseconds);
                        }

                        if (notification.UseBigPicture)
                        {
                            notifyData.BigPicture = new BigPictureStyle()
                            {
                                ContentDescription = notification.BigPicture.ContentDescription,
                                ContentTitle = notification.BigPicture.ContentTitle,
                                LargeIcon = notification.BigPicture.LargeIcon,
                                Picture = notification.BigPicture.Picture,
                                ShowWhenCollapsed = notification.BigPicture.ShowWhenCollapsed,
                                SummaryText = notification.BigPicture.SummaryText
                            };
                        }

                        /*if (notification.RequestForExactScheduling)
                        {
                            #if UNITY_ANDROID
                            var currentUserOS = Helper.GetSDKLevel();
                            if (currentUserOS > 30)
                            {
                                if (!Permission.HasUserAuthorizedPermission("SCHEDULE_EXACT_ALARM"))
                                {
                                    Permission.RequestUserPermission("SCHEDULE_EXACT_ALARM");
                                }

                                if (Permission.HasUserAuthorizedPermission("SCHEDULE_EXACT_ALARM"))
                                {
                                    if(!AndroidNotificationCenter.UsingExactScheduling)
                                        AndroidNotificationCenter.RequestExactScheduling();
                                }
                            }
                            /*else
                            { 
                                if(!Permission.HasUserAuthorizedPermission("USE_EXACT_ALARM"))
                                {
                                    Permission.RequestUserPermission("USE_EXACT_ALARM");    
                                }
                            }#1#
                            #endif
                        }

                        if (notification.RequestForIgnoringBatteryOptimizations)
                        {
                            #if UNITY_ANDROID
                            var currentUserOS = Helper.GetSDKLevel();
                            if (currentUserOS is > 22 and <= 30)
                            {
                                if (Permission.HasUserAuthorizedPermission("REQUEST_IGNORE_BATTERY_OPTIMIZATIONS"))
                                {
                                    if (!AndroidNotificationCenter.IgnoringBatteryOptimizations)
                                        AndroidNotificationCenter.RequestIgnoreBatteryOptimizations();
                                }
                                else
                                {
                                    Permission.RequestUserPermission("REQUEST_IGNORE_BATTERY_OPTIMIZATIONS");

                                    if (!AndroidNotificationCenter.IgnoringBatteryOptimizations)
                                        AndroidNotificationCenter.RequestIgnoreBatteryOptimizations();
                                }
                            }
                            #endif
                        }*/
                        
                        var notificationStatus = _androidNotifications.CheckScheduledNotificationStatus(notification.NotificationId);

                        switch (notificationStatus)
                        {
                            case NotificationStatus.Scheduled:
                                // Replace the scheduled notification with a new notification.
                                _androidNotifications.UpdateNotificationScheduling(notification.NotificationId, notifyData, notificationChannel.ChannelData.Id);
                                break;
                            case NotificationStatus.Delivered:
                                // Remove the previously shown notification from the status bar.
                                _androidNotifications.CancelNotification(notification.NotificationId);
                                break;
                            case NotificationStatus.Unknown:
                                _androidNotifications.SendNotification(notificationChannel.ChannelData.Id, notifyData, notification.NotificationId);
                                break;
                        }
                        
                    }
                }
            }
        }
        public void ScheduleNotification(string groupId, string channelId, string notificationName)
        {
            var group = _notificationData.FirstOrDefault(g => g.GroupData.Id == groupId);
            if (group == null)
            {
                Debug.Log("Group doesn't exists!");
                return;
            }
            
            var groupData = new AndroidNotificationChannelGroup()
            {
                Id = group.GroupData.Id,
                Name = group.GroupData.Name,
                Description = group.GroupData.Description
            };
            _androidNotifications.CreateAndRegisterGroup(groupData);
            var channel = group.Channels.FirstOrDefault(c => c.ChannelData.Id == channelId);
            if (channel == null)
            {
                Debug.Log("Channel doesn't exist!");
                return;
            }
            
            var channelData = new AndroidNotificationChannel()
            {
                Id = channel.ChannelData.Id,
                Name = channel.ChannelData.Name,
                Description = channel.ChannelData.Description,
                CanBypassDnd = channel.ChannelData.CanBypassDnd,
                CanShowBadge = channel.ChannelData.CanShowBadge,
                EnableLights = channel.ChannelData.EnableLights,
                EnableVibration = channel.ChannelData.EnableVibration,
                Group = group.GroupData.Id,
                Importance = channel.ChannelData.Importance,
                LockScreenVisibility = channel.ChannelData.LockScreenVisibility
            };
                    
            _androidNotifications.CreateAndRegisterCustomNotificationChannel(channelData);
            
            var notification = channel.NotificationsData.FirstOrDefault(n => n.NotificationName == notificationName);
            if (notification == null)
            {
                Debug.Log("Notification doesn't exist!");
                return;
            }
            
            var notifyData = new AndroidNotification()
            {
                Title = notification.NotificationTitle,
                Text = notification.NotificationText,
                Color = notification.HasColor ? notification.Color : null,
                FireTime = DateTime.Now.AddYears(notification.FireTime.Years).AddMonths(notification.FireTime.Months)
                    .AddDays(notification.FireTime.Days).AddHours(notification.FireTime.Hours).AddMinutes(notification.FireTime.Minutes)
                    .AddSeconds(notification.FireTime.Seconds).AddMilliseconds(notification.FireTime.Milliseconds),
                RepeatInterval = notification.HasRepeatInterval ? notification.RepeatInterval : null,
                Group = channel.Group,
                GroupSummary = channel.GroupSummary,
                GroupAlertBehaviour = channel.GroupAlertBehaviour,
                IntentData = notification.IntentData,
                UsesStopwatch = notification.UseStopWatch,
                SortKey = notification.SortKey,
                ShowTimestamp = notification.ShowTimestamp,
                ShowInForeground = notification.ShowInForeground,
                ShouldAutoCancel = notification.ShouldAutoCancel
            };

            if (notification.UseCustomTimestamp)
            {
                notifyData.CustomTimestamp = new DateTime(notification.CustomTimestamp.Years, 
                    notification.CustomTimestamp.Months, notification.CustomTimestamp.Days, 
                    notification.CustomTimestamp.Hours, notification.CustomTimestamp.Minutes,
                    notification.CustomTimestamp.Seconds, notification.CustomTimestamp.Milliseconds);
            }

            if (notification.UseBigPicture)
            {
                notifyData.BigPicture = new BigPictureStyle()
                {
                    ContentDescription = notification.BigPicture.ContentDescription,
                    ContentTitle = notification.BigPicture.ContentTitle,
                    LargeIcon = notification.BigPicture.LargeIcon,
                    Picture = notification.BigPicture.Picture,
                    ShowWhenCollapsed = notification.BigPicture.ShowWhenCollapsed,
                    SummaryText = notification.BigPicture.SummaryText
                };
            }
            
            var notificationStatus = _androidNotifications.CheckScheduledNotificationStatus(notification.NotificationId);

            switch (notificationStatus)
            {
                case NotificationStatus.Scheduled:
                    // Replace the scheduled notification with a new notification.
                    _androidNotifications.UpdateNotificationScheduling(notification.NotificationId, notifyData, channel.ChannelData.Id);
                    break;
                case NotificationStatus.Delivered:
                    // Remove the previously shown notification from the status bar.
                    _androidNotifications.CancelNotification(notification.NotificationId);
                    break;
                case NotificationStatus.Unknown:
                    _androidNotifications.SendNotification(channel.ChannelData.Id, notifyData, notification.NotificationId);
                    break;
            }
        }
        public void CancelAllNotifications()
        {
            for (var index = 0; index < _notificationData.Count; index++)
            {
                var groupData = _notificationData[index];
                for (var index1 = 0; index1 < groupData.Channels.Count; index1++)
                {
                    var channel = groupData.Channels[index1];
                    for (var index2 = 0; index2 < channel.NotificationsData.Count; index2++)
                    {
                        var notification = channel.NotificationsData[index2];
                        _androidNotifications.CancelNotification(notification.NotificationId);
                    }
                }
            }
        }
        public void CancelNotification(string groupId, string channelId, string notificationName)
        {
            var group = _notificationData.FirstOrDefault(g => g.GroupData.Id == groupId);
            if (group == null)
            {
                Debug.Log("Group doesn't exists!");
                return;
            }

            var channel = group.Channels.FirstOrDefault(c => c.ChannelData.Id == channelId);
            if (channel == null)
            {
                Debug.Log("Channel doesn't exist!");
                return;
            }
            
            var notification = channel.NotificationsData.FirstOrDefault(n => n.NotificationName == notificationName);
            if (notification == null)
            {
                Debug.Log("Notification doesn't exist!");
                return;
            }
            
            _androidNotifications.CancelNotification(notification.NotificationId);
        }
    }

    [Serializable]
    public class AndroidNotificationGroupData
    {
        public AndroidNotificationChannelGroupData GroupData;
        public List<AndroidNotificationChannelsData> Channels;
        
    }

    [Serializable]
    public class AndroidNotificationChannelsData
    {
        [Tooltip("Every Channel")]
        public AndroidNotificationChannelData ChannelData;
        
        [Tooltip("Set this property for the notification to be made part of a group of notifications sharing the same key." +
                 "Grouped notifications may display in a cluster or stack on devices which support such rendering." +
                 "Only available on Android 7.0 (API level 24) and above.")]
        public string Group;

        /// <summary>
        /// Set this notification to be the group summary for a group of notifications. Requires the 'Group' property to also be set.
        /// Grouped notifications may display in a cluster or stack on devices which support such rendering.
        /// Only available on Android 7.0 (API level 24) and above.
        /// </summary>
        [Tooltip("Set this notification to be the group summary for a group of notifications. Requires the 'Group' property to also be set." +
                 "Grouped notifications may display in a cluster or stack on devices which support such rendering." +
                 "Only available on Android 7.0 (API level 24) and above.")]
        public bool GroupSummary;

        /// <summary>
        /// Sets the group alert behavior for this notification. Set this property to mute this notification if alerts for this notification's group should be handled by a different notification.
        /// This is only applicable for notifications that belong to a group. This must be set on all notifications you want to mute.
        /// Only available on Android 8.0 (API level 26) and above.
        /// </summary>
        [Tooltip("Sets the group alert behavior for this notification. Set this property to mute this notification if alerts for this notification's group should be handled by a different notification." +
                 "This is only applicable for notifications that belong to a group. This must be set on all notifications you want to mute." +
                 "Only available on Android 8.0 (API level 26) and above.")]
        public GroupAlertBehaviours GroupAlertBehaviour;
        
        public List<AndroidNotificationData> NotificationsData;
    }
    
    [Serializable]
    public class AndroidNotificationChannelGroupData
    {
        /// <summary>
        /// A unique ID for this group. Will rename the group if already exists.
        /// </summary>
        public string Id;

        /// <summary>
        /// A user visible name for this group.
        /// </summary>
        public string Name;

        /// <summary>
        /// A description for this group.
        /// </summary>
        public string Description;
    }
    
    [Serializable]
    public class AndroidNotificationChannelData
    {
        /// <summary>
        /// Notification channel identifier.
        /// Must be specified when scheduling notifications.
        /// </summary>
        public string Id;

        /// <summary>
        /// Notification channel name which is visible to users.
        /// </summary>
        public string Name;

        /// <summary>
        /// User visible description of the notification channel.
        /// </summary>
        public string Description;
        
        /// <summary>
        /// Importance level which is applied to all notifications sent to the channel.
        /// This can be changed by users in the settings app. Android uses importance to determine how much the notification should interrupt the user (visually and audibly).
        /// The higher the importance of a notification, the more interruptive the notification will be.
        /// The possible importance levels are the following:
        ///    High: Makes a sound and appears as a heads-up notification.
        ///    Default: Makes a sound.
        ///    Low: No sound.
        ///    None: No sound and does not appear in the status bar.
        /// </summary>
        public Importance Importance;

        /// <summary>
        /// Whether or not notifications posted to this channel can bypass the Do Not Disturb.
        /// This can be changed by users in the settings app.
        /// </summary>
        public bool CanBypassDnd;

        /// <summary>
        /// Whether notifications posted to this channel can appear as badges in a Launcher application.
        /// </summary>
        public bool CanShowBadge;

        /// <summary>
        /// Sets whether notifications posted to this channel should display notification lights, on devices that support that feature.
        /// This can be changed by users in the settings app.
        /// </summary>/
        public bool EnableLights;

        /// <summary>
        /// Sets whether notification posted to this channel should vibrate.
        /// This can be changed by users in the settings app.
        /// </summary>
        public bool EnableVibration;

        /// <summary>
        /// Sets the vibration pattern for notifications posted to this channel.
        /// </summary>
        public long[] VibrationPattern;

        /// <summary>
        /// Sets whether or not notifications posted to this channel are shown on the lockscreen in full or redacted form.
        /// This can be changed by users in the settings app.
        /// </summary>
        public LockScreenVisibility LockScreenVisibility;
    }
    
    [Serializable]
    public class AndroidNotificationData
    {
        /*[Tooltip("Google recommends applications that require exact scheduling as a key feature to declare SCHEDULE_EXACT_ALARM and USE_EXACT_ALARM permissions. " +
                 "Applications that do not require exact scheduling and still declare these permissions are prohibited to publish on Google Play. For more information, " +
                 "refer to Google's policy on restricted permission requirement for Exact Alarm API.")]
        public bool RequestForExactScheduling;
        
        [Tooltip("Whether app should ignore device battery optimization settings." +
                 "When device is in power saving or similar restricted mode, scheduled notifications may not appear or be late")]
        public bool RequestForIgnoringBatteryOptimizations;*/
        
        [Tooltip("Just for query puposes.")]
        public string NotificationName;
        
        //Notifications have there own Ids but you can assign them custom Ids.
        [Tooltip("Notifications have there own Ids but you have to assign them custom Ids," +
                 " so we can tract them for future purposes.")]
        public int NotificationId;
        
        /// <summary>
        /// Notification title.
        /// Set the first line of text in the notification.
        /// </summary>
        [Tooltip("Notification title. Set the first line of text in the notification.")]
        public string NotificationTitle;
        
        /// <summary>
        /// Notification body.
        /// Set the second line of text in the notification.
        /// </summary>
        [Tooltip("Notification body. Set the second line of text in the notification.")]
        public string NotificationText;
        
        /// <summary>
        /// Notification small icon.
        /// It will be used to represent the notification in the status bar and content view (unless overridden there by a large icon)
        /// The icon has to be registered in Notification Settings or a PNG file has to be placed in the `res/drawable` folder of the Android library plugin
        /// and it's name has to be specified without the extension.
        /// Alternatively it can also be URI supported by the OS.
        /// </summary>
        [Tooltip("Notification small icon. It will be used to represent the notification in the status bar and content view (unless overridden there by a large icon) " +
                 "The icon has to be registered in Notification Settings or a PNG file has to be placed in the `res/drawable` folder of the Android library plugin and it's name has to be specified without the extension. " +
                 "Alternatively it can also be URI supported by the OS.")]
        public string SmallIcon;
        
        /// <summary>
        /// Notification large icon.
        /// Add a large icon to the notification content view. This image will be shown on the left of the notification view in place of the small icon (which will be placed in a small badge atop the large icon).
        /// The icon has to be registered in Notification Settings or a PNG file has to be placed in the `res/drawable` folder of the Android library plugin
        /// and it's name has to be specified without the extension.
        /// Alternatively it can be a file path or system supported URI.
        /// </summary>
        [Tooltip("Notification large icon. Add a large icon to the notification content view. " +
                 "This image will be shown on the left of the notification view in place of the small icon (which will be placed in a small badge atop the large icon)." +
                 "The icon has to be registered in Notification Settings or a PNG file has to be placed in the `res/drawable` folder of the Android library plugin and it's name has to be specified without the extension. " +
                 "Alternatively it can be a file path or system supported URI.")]
        public string LargeIcon;
        
        /// <summary>
        /// The date and time when the notification should be delivered.
        /// </summary>
        [Tooltip("The date and time when the notification should be delivered.")]
        public DateTimeData FireTime;

        public bool HasRepeatInterval;
        
        /// <summary>
        /// The notification will be be repeated on every specified time interval.
        /// Do not set for one time notifications.
        /// </summary>
        [Tooltip("The notification will be be repeated on every specified time interval." +
                 "Do not set for one time notifications.")]
        public UTimeSpan RepeatInterval;
        
        /// <summary>
        /// Apply a custom style to the notification.
        /// Currently only BigPicture and BigText styles are supported.
        /// </summary>
        [Tooltip("Apply a custom style to the notification." +
                 "Currently only BigPicture and BigText styles are supported.")]
        public NotificationStyle NotificationStyle;

        public bool HasColor;
        
        /// <summary>
        /// Accent color to be applied by the standard style templates when presenting this notification.
        /// The template design constructs a colorful header image by overlaying the icon image (stenciled in white) atop a field of this color. Alpha components are ignored.
        /// </summary>
        [Tooltip("Accent color to be applied by the standard style templates when presenting this notification." +                                                                     
            "The template design constructs a colorful header image by overlaying the icon image (stenciled in white) atop a field of this color. Alpha components are ignored.")]
        public Color Color = Color.black;
        
        /// <summary>
        /// Sets the number of items this notification represents.
        /// Is displayed as a badge count on the notification icon if the launcher supports this behavior.
        /// </summary>
        [Tooltip("Sets the number of items this notification represents." +
                 "Is displayed as a badge count on the notification icon if the launcher supports this behavior.")]
        public int Number;
        
        /// <summary>
        /// This notification will automatically be dismissed when the user touches it.
        /// By default this behavior is turned off.
        /// </summary>
        [Tooltip("This notification will automatically be dismissed when the user touches it." +
                 "By default this behavior is turned off.")]
        public bool ShouldAutoClose;
        
        /// <summary>
        /// Show the notification time field as a stopwatch instead of a timestamp.
        /// </summary>
        [Tooltip("Show the notification time field as a stopwatch instead of a timestamp.")]
        public bool UseStopWatch;

        /// <summary>
        /// The sort key will be used to order this notification among other notifications from the same package.
        /// Notifications will be sorted lexicographically using this value.
        /// </summary>
        [Tooltip("The sort key will be used to order this notification among other notifications from the same package." +
                 "Notifications will be sorted lexicographically using this value.")]
        public string SortKey;

        /// <summary>
        /// Use this to save arbitrary string data related to the notification.
        /// </summary>
        [Tooltip("Use this to save arbitrary string data related to the notification i.e " +
                 "\"{\\\"title\\\": \\\"Notification 1\\\", \\\"data\\\": \\\"200\\\"}\"")]
        public string IntentData;

        /// <summary>
        /// Enable it to show a timestamp on the notification when it's delivered, unless the "CustomTimestamp" property is set "FireTime" will be shown.
        /// </summary>
        [Tooltip("Enable it to show a timestamp on the notification when it's delivered, unless the \"CustomTimestamp\" property is set \"FireTime\" will be shown.")]
        public bool ShowTimestamp;

        public bool UseCustomTimestamp;
        
        /// <summary>
        /// Set this to show custom date instead of the notification's "FireTime" as the notification's timestamp'.
        /// </summary>
        [Tooltip("Set this to show custom date instead of the notification's \"FireTime\" as the notification's timestamp'.")]
        public DateTimeData CustomTimestamp;

        /// <summary>
        /// Set this notification to be shown when app is in the foreground (default: true).
        /// </summary>
        [Tooltip("Set this notification to be shown when app is in the foreground (default: true).")]
        public bool ShowInForeground;

        /// <summary>
        /// This notification will automatically be dismissed when the user touches it.
        /// By default this behavior is turned off.
        /// </summary>
        [Tooltip("This notification will automatically be dismissed when the user touches it." +
                 "By default this behavior is turned off.")]
        public bool ShouldAutoCancel;
        
        public bool UseBigPicture;
        
        /// <summary>
        /// The necessary properties for big picture style notification.
        /// For convenience, assigning this property will also set the Style property.
        /// </summary>
        [Tooltip("The necessary properties for big picture style notification." +
                 "For convenience, assigning this property will also set the Style property.")]
        public BigPictureStyleData BigPicture;
    }

    [Serializable]
    public class BigPictureStyleData
    {
        /// <summary>
        /// The override for large icon (requirements are the same).
        /// </summary>
        /// <see cref="AndroidNotification.LargeIcon"/>
        public string LargeIcon;

        /// <summary>
        /// The picture to be displayed.
        /// Can be resource name (like icon), file path or an URI supported by Android.
        /// </summary>
        public string Picture;

        /// <summary>
        /// The content title to be displayed in the notification.
        /// </summary>
        public string ContentTitle;

        /// <summary>
        /// The content description to set.
        /// </summary>
        public string ContentDescription;

        /// <summary>
        /// The summary text to be shown.
        /// </summary>
        public string SummaryText;

        /// <summary>
        /// Whether to show big picture in place of large icon when collapsed.
        /// </summary>
        public bool ShowWhenCollapsed;
    }
}
