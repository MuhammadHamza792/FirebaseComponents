using System;
using System.Collections.Generic;
using System.Linq;
using Firebase;
using Firebase.Auth;
using Google;
using UnityEngine;

namespace _Project.Scripts.Authentication.Firebase
{
    public class FirebaseGoogle : MonoBehaviour, ISignInGoogle
    {
        [SerializeField] private string _googleWebApi;

        private GoogleSignInConfiguration _configuration;

        private void Start()
        {
            _configuration = new GoogleSignInConfiguration()
            {
                WebClientId = _googleWebApi,
                RequestIdToken = true,
                RequestEmail = true,
                UseGameSignIn = false
            };
        }

        public async void GoogleSignIn(Action<AuthenticationData> onComplete = null, Action<string> onFailure = null)
        {
            var firebaseAuthenticator = FirebaseAuthenticatorHandler.Instance;
            Google.GoogleSignIn.Configuration = _configuration;
            Credential credential = null;
            
            var isFaulted = false;
            var faultMsg = string.Empty;
            var authenticationData = new AuthenticationData();
            
            await Google.GoogleSignIn.DefaultInstance.SignIn().ContinueWith(task =>
            {
                if (task.IsCanceled) {
                    isFaulted = true;
                    faultMsg = "Login was canceled";
                    Debug.LogError("Login was canceled");
                    return;
                }
                if (task.IsFaulted) {
                    isFaulted = true;
                    faultMsg = "Encountered an error: " + task.Exception;
                    Debug.LogError("Encountered an error: " + task.Exception);
                    return;
                }

                var googleIdToken = task.Result.IdToken;
                credential = GoogleAuthProvider.GetCredential(googleIdToken, null);
            });
            
            if (isFaulted)
            {
                onFailure?.Invoke(faultMsg);
                return;
            }
            
            if (credential == null)
            {
                onFailure?.Invoke("Failed to get credential");
                Debug.LogError("Failed to get credential");
                return;
            }
            
            await firebaseAuthenticator.Auth.SignInAndRetrieveDataWithCredentialAsync(credential).ContinueWith(task => {
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
                return;
            }
            
            onComplete?.Invoke(authenticationData);
            
        }

        public async void LinkToGoogle(Action<AuthenticationData> onComplete = null, Action<string> onFailure = null)
        {
            var firebaseAuthenticator = FirebaseAuthenticatorHandler.Instance;
            var auth = firebaseAuthenticator.Auth;

            var isFaulted = false;
            var faultErrorMsg = string.Empty;
            var authenticationData = new AuthenticationData();
            Credential credential = null;

            Google.GoogleSignIn.Configuration = _configuration;
            await Google.GoogleSignIn.DefaultInstance.SignIn().ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    isFaulted = true;
                    faultErrorMsg = "Login was canceled.";
                    Debug.LogError($"Login was canceled");
                    return;
                }

                if (task.IsFaulted)
                {
                    isFaulted = true;
                    faultErrorMsg = "Encountered an error: " + task.Exception;
                    Debug.LogError("Encountered an error: " + task.Exception);
                    return;
                }

                var googleIdToken = task.Result.IdToken;
                credential = GoogleAuthProvider.GetCredential(googleIdToken, null);
                
            });

            if (isFaulted)
            {
                onFailure?.Invoke(faultErrorMsg);
                return;
            }
            
            if (credential == null)
            {
                onFailure?.Invoke("Failed to get credential");
                Debug.LogError("Failed to get credential");
                return;
            }
            
            await auth.CurrentUser.LinkWithCredentialAsync(credential).ContinueWith(task =>
                {
                    if (task.IsCanceled)
                    {
                        isFaulted = true;
                        faultErrorMsg = "Linking with google was canceled.";
                        Debug.LogError("Linking With google was canceled.");
                        return;
                    }

                    if (task.IsFaulted)
                    {
                        /*if (task.Exception?.InnerException is FirebaseException firebaseException)
                        {
                            Debug.LogError("While linking with google encountered an error: " + task.Exception);
                            
                            var authError = ((AuthError)firebaseException.ErrorCode);

                            if (authError == AuthError.AccountExistsWithDifferentCredentials)
                            {
                                Debug.Log("Merging both accounts.");
                                
                                var currentUserId = auth.CurrentUser.UserId;
                                var currentEmail = auth.CurrentUser.Email;
                                var currentDisplayName = auth.CurrentUser.DisplayName;
                                var currentPhotoUrl = auth.CurrentUser.PhotoUrl;

                                // Sign in with the new credentials.
                                auth.SignInAndRetrieveDataWithCredentialAsync(credential).ContinueWith(mergeTask => {
                                    if (mergeTask.IsCanceled) {
                                        onFailure?.Invoke("Failed to Merging both accounts.");
                                        Debug.LogError("SignInAndRetrieveDataWithCredentialAsync was canceled.");
                                        return;
                                    }
                                    if (mergeTask.IsFaulted) {
                                        onFailure?.Invoke("Failed to Merging both accounts.");
                                        Debug.LogError("SignInAndRetrieveDataWithCredentialAsync encountered an error: " + mergeTask.Exception);
                                        return;
                                    }

                                    var result = mergeTask.Result;
                                    Debug.LogFormat("User signed in successfully: {0} ({1})",
                                        result.User.DisplayName, result.User.UserId);

                                    // TODO: Merge app specific details using the newUser and values from the
                                    // previous user, saved above.
                                    firebaseAuthenticator.UpdateAuthenticatedData(currentDisplayName, currentPhotoUrl.ToString(),
                                        () =>
                                        {
                                            onComplete?.Invoke();
                                        },()=>
                                        {
                                            onFailure?.Invoke("Failed to update previous account's details");
                                        });
                                });
                            }
                        }*/
                        
                        isFaulted = true;
                        faultErrorMsg = "While linking with google encountered an error: " + task.Exception;
                        Debug.LogError("While linking with google encountered an error: " + task.Exception);
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

        public async void UnlinkGoogleAccount(Action<AuthenticationData> onComplete = null, Action<string> onFailure = null)
        {
            var firebaseAuthenticator = FirebaseAuthenticatorHandler.Instance;
            var auth = firebaseAuthenticator.Auth;

            var hasGoogleProvider = firebaseAuthenticator.Providers.Contains(FirebaseProviders.GoogleProvider);
            
            if (!hasGoogleProvider)
            {
                onFailure?.Invoke("This Email is not Linked to Google");
                Debug.LogError("This Email is not Linked to Google");
                return;
            }

            var isFaulted = false;
            var faultedMsg = "";
            var authenticationData = new AuthenticationData();
            
            await auth.CurrentUser.UnlinkAsync(FirebaseProviders.GoogleProvider).ContinueWith(unlinkingTask =>
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