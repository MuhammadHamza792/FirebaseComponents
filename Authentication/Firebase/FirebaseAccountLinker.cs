using System;
using System.Collections.Generic;
using System.Linq;
using Firebase.Auth;
using UnityEngine;

namespace _Project.Scripts.Authentication.Firebase
{
    public class FirebaseAccountLinker : MonoBehaviour, ILinker
    {
        public async void LinkAccount(string email, string pass, Action<AuthenticationData> onComplete = null, Action<string> onFailure = null)
        {
            var firebaseAuthenticator = FirebaseAuthenticatorHandler.Instance;
            var auth = firebaseAuthenticator.Auth;

            var isFaulted = false;
            var faultErrorMsg = string.Empty;
            var authenticationData = new AuthenticationData();
            Credential credential = null;

            await auth.SignInWithEmailAndPasswordAsync(email, pass).ContinueWith(signInTask =>
            {
                if (signInTask.IsCanceled)
                {
                    isFaulted = true;
                    faultErrorMsg = "Sign in was canceled.";
                    Debug.LogError("Sign in was canceled.");
                    return;
                }

                if (signInTask.IsFaulted)
                {
                    isFaulted = true;
                    faultErrorMsg = "While signing in encountered an error: " + signInTask.Exception;
                    Debug.LogError("While signing in encountered an error: " + signInTask.Exception);
                    return;
                }

                credential = signInTask.Result.Credential;
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
                        faultErrorMsg = "Linking with Firebase was canceled.";
                        Debug.LogError("Linking With Firebase was canceled.");
                        return;
                    }

                    if (task.IsFaulted)
                    {
                        /*if (task.Exception?.InnerException is FirebaseException firebaseException)
                        {
                            Debug.LogError("While linking with Firebase encountered an error: " + task.Exception);
                            
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
                        faultErrorMsg = "While linking with Firebase encountered an error: " + task.Exception;
                        Debug.LogError("While linking with Firebase encountered an error: " + task.Exception);
                        return;
                    }

                    var result = task.Result;
                    
                    // TODO: You can get all the data from the link account.
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

        public async void UnlinkAccount(Action<AuthenticationData> onComplete = null, Action<string> onFailure = null)
        {
            var firebaseAuthenticator = FirebaseAuthenticatorHandler.Instance;
            var auth = firebaseAuthenticator.Auth;

            var hasFirebaseProvider = firebaseAuthenticator.Providers.Contains(FirebaseProviders.CustomProvider);
            
            if (!hasFirebaseProvider)
            {
                onFailure?.Invoke("This Email is not Linked to Custom Email");
                Debug.LogError("This Email is not Linked to Custom Email");
                return;
            }

            var isFaulted = false;
            var faultedMsg = "";
            var authenticationData = new AuthenticationData();
            
            await auth.CurrentUser.UnlinkAsync(FirebaseProviders.CustomProvider).ContinueWith(unlinkingTask =>
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
