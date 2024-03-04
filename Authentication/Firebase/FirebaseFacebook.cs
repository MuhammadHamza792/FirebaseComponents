//Commented Because Sdk is not installed.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Facebook.Unity;
using Firebase;
using Firebase.Auth;
using UnityEngine;

namespace _Project.Scripts.Authentication.Firebase
{
    public class FirebaseFacebook : MonoBehaviour , ISignInFacebook
    {
        public string Token;
        public string Error;
        private List<string> _public_email_Perms;

        private void InitCallback ()
        {
            if (FB.IsInitialized) {
                // Signal an app activation App Event
                FB.ActivateApp();
                // Continue with Facebook SDK
            } else {
                Debug.Log("Failed to Initialize the Facebook SDK");
            }
        }

        private void OnHideUnity (bool isGameShown) =>
            // Pause the game - we will need to hide
            Time.timeScale = !isGameShown ? 0 :
                // Resume the game - we're getting focus again
                1;

        void AuthCallback (ILoginResult result) {
            if (FB.IsLoggedIn) {
                // AccessToken class will have session details
                var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
                // Print current access token's User ID
                Debug.Log(aToken.UserId);
                // Print current access token's granted permissions
                foreach (string perm in aToken.Permissions) {
                    Debug.Log(perm);
                }
            } else {
                Debug.Log("User cancelled login");
            }
        }
        
        void LoginStatusCallback(ILoginStatusResult result) {
            if (!string.IsNullOrEmpty(result.Error)) {
                Debug.Log("Error: " + result.Error);
            } else if (result.Failed) {
                Debug.Log("Failure: Access Token could not be retrieved");
            } else {
                // Successfully logged user in
                // A popup notification will appear that says "Logged in as <User Name>"
                Debug.Log("Success: " + result.AccessToken.UserId);
            }
        }
        
        public void FacebookSignIn(Action<AuthenticationData> onComplete = null, Action<string> onFailure = null)
        {
            if (!FB.IsInitialized) {
                // Initialize the Facebook SDK
                FB.Init(InitCallback, OnHideUnity);
            } else {
                // Already initialized, signal an app activation App Event
                FB.ActivateApp();
            }
            
            if (!FB.IsInitialized)
            {
                Debug.LogError("Failed To Initialize Facebook.");
                onFailure?.Invoke("Failed To Initialize Facebook.");
                return;
            }
            
            // Define the permissions
            _public_email_Perms ??= new List<string>() {"public_profile", "email"};
            
            FB.LogInWithReadPermissions(_public_email_Perms, result =>
            {
                if (FB.IsLoggedIn)
                {
                    Token = AccessToken.CurrentAccessToken.TokenString;
                    Debug.Log($"Facebook Login token: {Token}");
                    ConnectToFirebase(Token, onComplete, onFailure);
                }
                else
                {
                    Error = "User cancelled login";
                    Debug.Log("[Facebook Login] User cancelled login");
                    onFailure?.Invoke(Error);
                }
            });
        }

        private async void ConnectToFirebase(string token, Action<AuthenticationData> onComplete = null, Action<string> onFailure = null)
        {
            var credential = FacebookAuthProvider.GetCredential(token);
            var auth = FirebaseAuthenticatorHandler.Instance.Auth;
            
            var isFaulted = false;
            var faultMsg = string.Empty;
            var authenticationData = new AuthenticationData();
            
            await auth.SignInAndRetrieveDataWithCredentialAsync(credential).ContinueWith(task => {
                if (task.IsCanceled) {
                    isFaulted = true;
                    faultMsg = "SignInAndRetrieveDataWithCredentialAsync was canceled.";
                    Debug.LogError("SignInAndRetrieveDataWithCredentialAsync was canceled.");
                    return;
                }
                if (task.IsFaulted) {
                    isFaulted = true;
                    faultMsg = "SignInAndRetrieveDataWithCredentialAsync encountered an error: " + task.Exception;
                    Debug.LogError("SignInAndRetrieveDataWithCredentialAsync encountered an error: " + task.Exception);
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
                Debug.LogError(faultMsg);
                return;
            }
            
            onComplete?.Invoke(authenticationData);
        }

        public void LinkToFacebook(Action<AuthenticationData> onComplete = null, Action<string> onFailure = null)
        {
            if (!FB.IsInitialized) {
                // Initialize the Facebook SDK
                FB.Init(InitCallback, OnHideUnity);
            } else {
                // Already initialized, signal an app activation App Event
                FB.ActivateApp();
            }
            
            if (!FB.IsInitialized)
            {
                Debug.LogError("Failed To Initialize Facebook.");
                onFailure?.Invoke("Failed To Initialize Facebook.");
                return;
            }
            
            // Define the permissions
            var perms = new List<string>() {"public_profile", "email"};
            
            FB.LogInWithReadPermissions(perms, result =>
            {
                if (FB.IsLoggedIn)
                {
                    Token = AccessToken.CurrentAccessToken.TokenString;
                    Debug.Log($"Facebook Login token: {Token}");
                    LinkWithFirebase(Token, onComplete, onFailure);
                }
                else
                {
                    Error = "User cancelled login";
                    Debug.Log("[Facebook Login] User cancelled login");
                    onFailure?.Invoke(Error);
                }
            });
        }

        private async void LinkWithFirebase(string token, Action<AuthenticationData> onComplete, Action<string> onFailure)
        {
            var credential = FacebookAuthProvider.GetCredential(token);
            var auth = FirebaseAuthenticatorHandler.Instance.Auth;

            var isFaulted = false;
            var faultErrorMsg = string.Empty;
            var authenticationData = new AuthenticationData();

            await auth.CurrentUser.LinkWithCredentialAsync(credential).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    isFaulted = true;
                    faultErrorMsg = "Linking with Facebook was canceled.";
                    Debug.LogError("Linking With Facebook was canceled.");
                    return;
                }

                if (task.IsFaulted)
                {
                    isFaulted = true;
                    faultErrorMsg = "While linking with Facebook encountered an error: " + task.Exception;
                    Debug.LogError("While linking with Facebook encountered an error: " + task.Exception);
                    return;
                }

                var result = task.Result;
                authenticationData.UserId = result.User.UserId;
                authenticationData.EmailAddress = result.User.Email;
                authenticationData.DisplayName = result.User.DisplayName;
                authenticationData.Provider = result.User.ProviderId;
                authenticationData.PhotoUrl = result.User.PhotoUrl;
                authenticationData.IsAnonymous = result.User.IsAnonymous;
                var userInfo = result.User.ProviderData.ToList();
                var providerIds = new List<string>();
                for (int i = 0; i < userInfo.Count; i++)
                {
                    var providerId = userInfo[i].ProviderId;
                    providerIds.Add(providerId);
                }

                authenticationData.Providers = providerIds;
                Debug.LogFormat("Credentials successfully linked to Firebase user: {0} ({1})",
                    result.User.DisplayName, result.User.UserId);
            });

            if (isFaulted)
            {
                onFailure?.Invoke(faultErrorMsg);
                return;
            }

            onComplete?.Invoke(authenticationData);
        }

        public async void UnLinkFacebookAccount(Action<AuthenticationData> onComplete = null, Action<string> onFailure = null)
        {
            var firebaseAuthenticator = FirebaseAuthenticatorHandler.Instance;
            var auth = firebaseAuthenticator.Auth;

            var hasFacebookProvider = firebaseAuthenticator.Providers.Contains(FirebaseProviders.FacebookProvider);
            
            if (!hasFacebookProvider)
            {
                onFailure?.Invoke("This Email is not Linked to Facebook");
                Debug.LogError("This Email is not Linked to Facebook");
                return;
            }

            var isFaulted = false;
            var faultedMsg = "";
            var authenticationData = new AuthenticationData();
            
            await auth.CurrentUser.UnlinkAsync(FirebaseProviders.FacebookProvider).ContinueWith(unlinkingTask =>
            {
                if (unlinkingTask.IsCanceled)
                {
                    Debug.LogError("UnlinkAsync was canceled.");
                    isFaulted = true;
                    faultedMsg = "UnlinkAsync was canceled.";
                    return;
                }
            
                if (unlinkingTask.IsFaulted)
                {
                    Debug.LogError("UnlinkAsync encountered an error: " + unlinkingTask.Exception);
                    isFaulted = true;
                    faultedMsg = "UnlinkAsync encountered an error: " + unlinkingTask.Exception;
                    return;
                }
            
                // The user has been unlinked from the provider.
                var unlinkingResult = unlinkingTask.Result;
                
                authenticationData.UserId = unlinkingResult.User.UserId;
                authenticationData.EmailAddress = unlinkingResult.User.Email;
                authenticationData.DisplayName = unlinkingResult.User.DisplayName;
                authenticationData.Provider = unlinkingResult.User.ProviderId;
                authenticationData.PhotoUrl = unlinkingResult.User.PhotoUrl;
                authenticationData.IsAnonymous = unlinkingResult.User.IsAnonymous;
                var userInfo = unlinkingResult.User.ProviderData.ToList();
                var providerIds = new List<string>();
                for (int i = 0; i < userInfo.Count; i++)
                {
                    var providerId = userInfo[i].ProviderId;
                    providerIds.Add(providerId);
                }
                authenticationData.Providers = providerIds;
                
                Debug.LogFormat("Credentials successfully unlinked from user: {0} ({1})",
                    unlinkingResult.User.DisplayName, unlinkingResult.User.UserId);
            });

            if (isFaulted)
            {
                onFailure?.Invoke(faultedMsg);
                return;
            }
            
            onComplete?.Invoke(authenticationData);
        }
    }
}