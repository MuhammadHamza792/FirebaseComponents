using System;
using System.Collections.Generic;
using System.Linq;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Project.Scripts.Authentication.Firebase
{
    public class FirebaseAuthenticatorHandler : MonoBehaviour , IAuthenticator , IAuthenticationData
    {
        public FirebaseUser user { private set; get; }
        public string UserId { private set; get; }
        public string DisplayName { get; private set; }
        public string EmailAddress { get; private set; }
        public Uri PhotoUrl { get; private set; }
        public string Provider { get; private set; }
        public List<string> Providers { get; private set; }
        public bool IsAnonymous { get; private set; }
        public bool SignedIn { get; private set; }
        
        public FirebaseAuth Auth { private set; get; }
        
        public static FirebaseAuthenticatorHandler Instance { private set; get; }

        public event Action OnSignedIn; 
        public event Action OnSignedOut;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(Instance.gameObject);
                return;
            }

            Instance = this;
        }

        public void InitializeFirebaseAuthentication() 
        {
            Auth = FirebaseAuth.DefaultInstance;
            Auth.StateChanged += AuthStateChanged;
            SignedIn = user != Auth.CurrentUser && Auth.CurrentUser != null && Auth.CurrentUser.IsValid();
            Debug.Log("Authentication Initialized!");
        }
        
        void AuthStateChanged(object sender, System.EventArgs eventArgs)
        {
            List<IUserInfo> providers;
            if (Auth.CurrentUser == user)
            {
                if (user == null)
                {
                    OnSignedOut?.Invoke();
                    return;
                }

                IsAnonymous = user.IsAnonymous;
                UserId = user.UserId ?? "";
                DisplayName = user.DisplayName ?? "";
                EmailAddress = user.Email ?? "";
                PhotoUrl = user.PhotoUrl ?? default;
                Provider = user.ProviderId ?? "";
                Providers ??= new List<string>();
                if(Providers.Count > 0) Providers.Clear();
                providers = user.ProviderData.ToList();
                for (int i = 0; i < providers.Count; i++)
                {
                    var provider = providers[i].ProviderId;
                    Providers.Add(provider);
                }
                
                OnSignedIn?.Invoke();
                
                return;
            }
            
            SignedIn = user != Auth.CurrentUser && Auth.CurrentUser != null && Auth.CurrentUser.IsValid();
            
            if (!SignedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
                OnSignedOut?.Invoke();
                return;
            }
            
            user = Auth.CurrentUser;

            if (user == null) return;
            
            Debug.Log("Signed in: " + user.UserId);
            Debug.Log("Provider Id: " + user.ProviderId);
            
            IsAnonymous = user.IsAnonymous;
            UserId = user.UserId ?? "";
            DisplayName = user.DisplayName ?? "";
            EmailAddress = user.Email ?? "";
            PhotoUrl = user.PhotoUrl ?? default;
            Provider = user.ProviderId ?? "";
            Providers ??= new List<string>();
            if(Providers.Count > 0) Providers.Clear();
            providers = user.ProviderData.ToList();
            for (int i = 0; i < providers.Count; i++)
            {
                var provider = providers[i].ProviderId;
                Providers.Add(provider);
            }
            OnSignedIn?.Invoke();
        }
        
        public async void UpdateDisplayName(string displayName, Action onCompleted = null, Action<string> onFailed = null)
        {
            var currentUser = Auth.CurrentUser;
            if (currentUser == null) return;
            var profile = new UserProfile
            {
                DisplayName = displayName,
                PhotoUrl = currentUser.PhotoUrl,
            };

            var isFaulted = false;
            var faultMsg = string.Empty;
            
            await currentUser.UpdateUserProfileAsync(profile).ContinueWith(task => {
                if (task.IsCanceled)
                {
                    isFaulted = true;
                    faultMsg = "UpdateUserProfileAsync was canceled.";
                    Debug.LogError("UpdateUserProfileAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    isFaulted = true;
                    faultMsg = "UpdateUserProfileAsync encountered an error: " + task.Exception;
                    Debug.LogError("UpdateUserProfileAsync encountered an error: " + task.Exception);
                    return;
                }
                
                Debug.Log("User profile updated successfully.");
            });

            if (isFaulted)
            {
                onFailed?.Invoke(faultMsg);
                return;
            }
            
            onCompleted?.Invoke();
        }

        public async void UpdateProfileImage(string photoUrl, Action onCompleted = null, Action<string> onFailed = null)
        {
            var currentUser = Auth.CurrentUser;
            if (currentUser == null) return;
            var profile = new UserProfile
            {
                DisplayName = currentUser.DisplayName,
                PhotoUrl = new Uri(photoUrl),
            };
            
            var isFaulted = false;
            var faultMsg = string.Empty;
            
            await currentUser.UpdateUserProfileAsync(profile).ContinueWith(task => {
                if (task.IsCanceled)
                {
                    isFaulted = true;
                    faultMsg = "UpdateUserProfileAsync was canceled.";
                    Debug.LogError("UpdateUserProfileAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    isFaulted = true;
                    faultMsg = "UpdateUserProfileAsync encountered an error: " + task.Exception;
                    Debug.LogError("UpdateUserProfileAsync encountered an error: " + task.Exception);
                    return;
                }
                
                Debug.Log("User profile updated successfully.");
            });
            
            if (isFaulted)
            {
                onFailed?.Invoke(faultMsg);
                return;
            }
            
            onCompleted?.Invoke();
        }

        public async void UpdateAuthenticatedData(string displayName, string photoUrl, Action onCompleted = null, Action<string> onFailed = null)
        {
            var currentUser = Auth.CurrentUser;
            if (currentUser == null) return;
            var profile = new UserProfile
            {
                DisplayName = displayName,
                PhotoUrl = new Uri(photoUrl),
            };
            
            var isFaulted = false;
            var faultMsg = string.Empty;
            
            await currentUser.UpdateUserProfileAsync(profile).ContinueWith(task => {
                if (task.IsCanceled)
                {
                    isFaulted = true;
                    faultMsg = "UpdateUserProfileAsync was canceled.";
                    Debug.LogError("UpdateUserProfileAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    isFaulted = true;
                    faultMsg = "UpdateUserProfileAsync encountered an error: " + task.Exception;
                    Debug.LogError("UpdateUserProfileAsync encountered an error: " + task.Exception);
                    return;
                }
                
                Debug.Log("User profile updated successfully.");
            });
            
            if (isFaulted)
            {
                onFailed?.Invoke(faultMsg);
                return;
            }
            
            onCompleted?.Invoke();
        }
        
        public async void SignUp(string email, string password, Action<AuthenticationData> onComplete = null, Action<string> onFailure = null)
        {
            var isFaulted = false;
            var faultMsg = string.Empty;
            AuthenticationData authenticationData = new AuthenticationData();
            
            await Auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
                if (task.IsCanceled) {
                    isFaulted = true;
                    faultMsg = "CreateUserWithEmailAndPasswordAsync was canceled.";
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                    return;
                }
                if (task.IsFaulted) {
                    isFaulted = true;
                    faultMsg = "CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception;
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    return;
                }
                
                var result = task.Result;
                authenticationData = new AuthenticationData()
                {
                    UserId = result.User.UserId,
                    DisplayName = result.User.DisplayName,
                    EmailAddress = result.User.Email,
                    IsAnonymous = result.User.IsAnonymous,
                    PhotoUrl = result.User.PhotoUrl,
                    Provider = result.User.ProviderId
                };
                var userInfo = result.User.ProviderData.ToList();
                var providerIds = new List<string>();
                for (int i = 0; i < userInfo.Count; i++)
                {
                    var providerId = userInfo[i].ProviderId;
                    providerIds.Add(providerId);
                }
                authenticationData.Providers = providerIds;
                
                Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                    result.User.DisplayName, result.User.UserId);
            });
            
            if (isFaulted)
            {
                onFailure?.Invoke(faultMsg);
                return;
            }
            
            onComplete?.Invoke(authenticationData);
        }

        public async void SignIn(string email, string password, Action<AuthenticationData> onComplete = null, Action<string> onFailure = null)
        {
            var isFaulted = false;
            var faultMsg = string.Empty;
            AuthenticationData authenticationData = new AuthenticationData();
            
            await Auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
                if (task.IsCanceled) {
                    isFaulted = true;
                    faultMsg = "SignInWithEmailAndPasswordAsync was canceled.";
                    Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                    return;
                }
                if (task.IsFaulted) {
                    isFaulted = true;
                    faultMsg = "SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception;
                    Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    return;
                }

                var result = task.Result;
                authenticationData = new AuthenticationData()
                {
                    UserId = result.User.UserId,
                    DisplayName = result.User.DisplayName,
                    EmailAddress = result.User.Email,
                    IsAnonymous = result.User.IsAnonymous,
                    PhotoUrl = result.User.PhotoUrl,
                    Provider = result.User.ProviderId
                };
                var userInfo = result.User.ProviderData.ToList();
                var providerIds = new List<string>();
                for (int i = 0; i < userInfo.Count; i++)
                {
                    var providerId = userInfo[i].ProviderId;
                    providerIds.Add(providerId);
                }
                authenticationData.Providers = providerIds;
                
                Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                    result.User.DisplayName, result.User.UserId);
            });
            
            if (isFaulted)
            {
                onFailure?.Invoke(faultMsg);
                return;
            }
            
            onComplete?.Invoke(authenticationData);
        }
        
        public async void SignInGuest(Action<AuthenticationData> onComplete = null, Action<string> onFailure = null)
        {
            var isFaulted = false;
            var faultMsg = string.Empty;
            AuthenticationData authenticationData = new AuthenticationData();
            
            await Auth.SignInAnonymouslyAsync().ContinueWith(task => {
                if (task.IsCanceled) {
                    isFaulted = true;
                    faultMsg = "SignInAnonymouslyAsync was canceled.";
                    Debug.LogError("SignInAnonymouslyAsync was canceled.");
                    return;
                }
                if (task.IsFaulted) {
                    isFaulted = true;
                    faultMsg = "SignInAnonymouslyAsync encountered an error: " + task.Exception;
                    Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                    return;
                }

                var result = task.Result;
                authenticationData = new AuthenticationData()
                {
                    UserId = result.User.UserId,
                    DisplayName = result.User.DisplayName,
                    EmailAddress = result.User.Email,
                    IsAnonymous = result.User.IsAnonymous,
                    PhotoUrl = result.User.PhotoUrl,
                    Provider = result.User.ProviderId
                };
                var userInfo = result.User.ProviderData.ToList();
                var providerIds = new List<string>();
                for (int i = 0; i < userInfo.Count; i++)
                {
                    var providerId = userInfo[i].ProviderId;
                    providerIds.Add(providerId);
                }
                authenticationData.Providers = providerIds;
                
                Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                    result.User.DisplayName, result.User.UserId);
            });
            
            if (isFaulted)
            {
                onFailure?.Invoke(faultMsg);
                return;
            }
            
            onComplete?.Invoke(authenticationData);
        }
        
        public void SignOut() => Auth.SignOut();

        void OnDestroy()
        {
            Auth.StateChanged -= AuthStateChanged;
            Auth = null;
        }
    }
}