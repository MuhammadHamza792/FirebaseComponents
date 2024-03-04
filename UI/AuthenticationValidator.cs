using System;
using System.Collections.Generic;
using _Project.Scripts.Authentication;
using _Project.Scripts.Authentication.Firebase;
using _Project.Scripts.Database;
using _Project.Scripts.Notifications;
using Firebase.Database;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Project.Scripts.UI
{
    public class AuthenticationValidator : MonoBehaviour , INotifier
    {
        [SerializeField] private AuthenticationUI _authentication;
        [SerializeField] private ProfilingUI _profilingUI;
        [SerializeField] private TextMeshProUGUI _linkToGoogleText;
        [SerializeField] private TextMeshProUGUI _linkToFacebookText;
        [SerializeField] private Button _linkToGoogleButton;
        [SerializeField] private Button _linkToFacebookButton;
        
        private AuthenticationData _authenticationData;

        private void Start()
        {
            _linkToGoogleButton.onClick.AddListener(LinkOrUnlinkWithGoogle);
            _linkToFacebookButton.onClick.AddListener(LinkOrUnlinkWithFacebook);
        }
        
        private void OnEnable()
        {
            Authenticator.OnAuthenticationSuccessful += SetProfile;
            Authenticator.OnAuthenticationFailed += ShowAuthenticationForum;
            Authenticator.OnAccountLinked += SetProfile;
            Authenticator.OnAccountUnlinked += SetProfile;
        }
        
        private void ShowAuthenticationForum()
        {
            _profilingUI.gameObject.SetActive(false);
            _authentication.gameObject.SetActive(true);
        }

        private void LinkOrUnlinkWithGoogle()
        {
            if (_authenticationData.Providers.Count != 0 && 
                _authenticationData.Providers.Contains(FirebaseProviders.GoogleProvider))
            {
                Authenticator.Instance.UnlinkFromGoogle();
            }
            else
            {
                Authenticator.Instance.LinkToGoogle();
            }
        }
        
        private void LinkOrUnlinkWithFacebook()
        {
            if (_authenticationData.Providers.Count != 0 && 
                _authenticationData.Providers.Contains(FirebaseProviders.FacebookProvider))
            {
                Authenticator.Instance.UnlinkFromFacebook();
            }
            else
            {
                Authenticator.Instance.LinkToFacebook();
            }
        }

        private void SetProfile(AuthenticationData data)
        {
            _authenticationData = data;

            foreach (var provider in _authenticationData.Providers)
            {
                Debug.Log(provider);    
            }
            
            _authentication.gameObject.SetActive(false);
            _profilingUI.gameObject.SetActive(true);
            _profilingUI.SetProfileData(data);
            
            CheckLinkedStatus();
            
            if(DatabaseHandler.Instance == null) return;
            
            DatabaseHandler.Instance.RetrieveData<DataSnapshot>(data.UserId, "userId", "users", (userData) =>
            {
                Debug.Log("User Exists!");
                _profilingUI.SetUserData(userData);
            }, () =>
            {
                Debug.Log("Failed to retrieve user's data or user doesn't exist!");
                var userData = new UserData
                {
                    Name = data.DisplayName,
                    Email = data.EmailAddress,
                    Level = 0,
                    Progress = 0,
                    Coins = 0,
                    CreatedOn = DateTime.Now
                };
                DatabaseHandler.Instance.CreateNewEntry(data.UserId, userData, "users", () =>
                {
                    Debug.Log("New User created");
                    DatabaseHandler.Instance.RetrieveData<DataSnapshot>(data.UserId, "userId", "users", (newUserData) =>
                    {
                        Debug.Log("DataRetrieved");
                        _profilingUI.SetUserData(newUserData);
                    }, () =>
                    {
                        Debug.Log("Failed to retrieve user's data or user doesn't exist!");
                    });
                } ,()=>
                {
                    Debug.Log("Failed to create user");
                });
            });
            
        }

        private void CheckLinkedStatus()
        {
            _linkToGoogleText.SetText(
                $"{(_authenticationData.Providers.Count != 0 && _authenticationData.Providers.Contains(FirebaseProviders.GoogleProvider) ? "UnlinkFromGoogle" : "LinkToGoogle")}");
            _linkToFacebookText.SetText(
                $"{(_authenticationData.Providers.Count != 0 && _authenticationData.Providers.Count != 0 && _authenticationData.Providers.Contains(FirebaseProviders.FacebookProvider) ? "UnlinkFromFacebook" : "LinkToFacebook")}");
        }

        private void ShowEmailAndPassPopUp()
        {
            NotificationHelper.SendNotification(NotificationType.Info, "Enter Email and Password", 
                "Please Enter Email and Password. Make sure you're registered first.", this, NotifyCallType.Open);
        }
        
        private void OnDisable()
        {
            Authenticator.OnAuthenticationSuccessful -= SetProfile;
            Authenticator.OnAuthenticationFailed -= ShowAuthenticationForum;
            Authenticator.OnAccountLinked -= SetProfile;
            Authenticator.OnAccountUnlinked -= SetProfile;
        }

        private void OnDestroy()
        {
            _linkToGoogleButton.onClick.RemoveListener(LinkOrUnlinkWithGoogle);
            _linkToFacebookButton.onClick.RemoveListener(LinkOrUnlinkWithFacebook);
        }

        public void Notify(string notifyData = null, bool accepted = false, bool rejected = false)
        {
            if (!accepted) return;
            if (notifyData == null) return;
            var notificationData = notifyData.Split(",");
            if (notificationData.Length == 2)
            {
                var email = notificationData[0];
                var pass = notificationData[1];
                Authenticator.Instance.LinkToCustomAccount(email, pass);
            }
        }
    }
}
