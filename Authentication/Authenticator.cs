using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Authentication
{
    public class Authenticator : MonoBehaviour
    {
        private IAuthenticationData _authenticationData;
        private IAuthenticator _authenticator;
        private ISignInGoogle _authenticatorGoogle;
        private ISignInFacebook _authenticatorFacebook;
        private ILinker _linker;
        
        private AuthenticationData _authenticatedData;

        public static event Action<AuthenticationData> OnAuthenticationSuccessful;
        public static event Action OnAuthenticationFailed;

        public static event Action OnSigningIn;
        public static event Action<AuthenticationData> OnSignedIn;
        public static event Action<string> OnFailedToSignIn;
        
        public static event Action OnSigningUp;
        public static event Action<AuthenticationData> OnSignedUp;
        public static event Action<string> OnFailedToSignUp;
        
        public static event Action OnLinkingAccount;
        public static event Action<AuthenticationData> OnAccountLinked;
        public static event Action<string> OnAccountFailedToLink;
        
        public static event Action OnUnlinkingAccount;
        public static event Action<AuthenticationData> OnAccountUnlinked;
        public static event Action<string> OnAccountFailedToUnlink;
        
        public static event Action OnSigningOut;
        public static event Action OnSignedOut;
        public static event Action<string> OnFailedToSignOut;
        
        public static event Action OnUpdatingInfo;
        public static event Action<AuthenticationData> OnInfoUpdated;
        public static event Action<string> OnFailedToUpdateInfo;
        
        public static event Action<AuthenticationData> OnAuthenticatedDataChanged;
        
        public bool SignedIn => _authenticationData?.SignedIn ?? false;
        
        public static Authenticator Instance { private set; get; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            _authenticator = GetComponent<IAuthenticator>();
            _authenticatorGoogle = GetComponent<ISignInGoogle>();
            _authenticatorFacebook = GetComponent<ISignInFacebook>();
            _authenticationData = GetComponent<IAuthenticationData>();
            _linker = GetComponent<ILinker>();
        }

        private void OnEnable()
        {
            _authenticationData.OnSignedIn += LoggedIn;
            _authenticationData.OnSignedOut += FailedToLogInOrHasLoggedOut;

            OnAccountLinked += UpdateAuthenticatedData;
            OnAccountUnlinked += UpdateAuthenticatedData;
        }

        private void LoggedIn()
        {
            var authenticatedData = new AuthenticationData()
            {
                IsAnonymous = _authenticationData.IsAnonymous,
                UserId = _authenticationData.UserId,
                DisplayName = _authenticationData.DisplayName,
                EmailAddress = _authenticationData.EmailAddress,
                PhotoUrl = _authenticationData.PhotoUrl,
                Provider = _authenticationData.Provider,
                Providers = _authenticationData.Providers
            };
            UpdateAuthenticatedData(authenticatedData);
            OnAuthenticationSuccessful?.Invoke(_authenticatedData);
        }

        private void FailedToLogInOrHasLoggedOut() => OnAuthenticationFailed?.Invoke();

        #region SignIn/SignOut

        private bool _signingIn;

        private void SignInWithCredentials(string email, string pass)
        {
            if (_signingIn) return;
            _signingIn = _authenticator != null;
            OnSigningIn?.Invoke();
            _authenticator?.SignIn(email, pass,
                (authenticationData) =>
                {
                    _signingIn = false;
                    OnSignedIn?.Invoke(authenticationData);
                }, (exc) =>
                {
                    _signingIn = false;
                    OnFailedToSignIn?.Invoke(exc);
                });
        }

        private bool _signingInAsGuest;

        private void SignInAsGuest()
        {
            if(_signingInAsGuest) return;
            _signingInAsGuest = _authenticator != null;
            OnSigningIn?.Invoke();
            _authenticator?.SignInGuest(
                (authenticationData) =>
                {
                    _signingInAsGuest = false;
                    OnSignedIn?.Invoke(authenticationData);
                }, (exc) =>
                { 
                    _signingInAsGuest = false;  
                    OnFailedToSignIn?.Invoke(exc);
                });
        }
        
        private bool _signingUp;

        private void SignUp(string email, string pass)
        {
            if (_signingUp) return;
            _signingUp = _authenticator != null;
            OnSigningUp?.Invoke();
            _authenticator?.SignUp(email, pass, 
                (authenticationData) =>
                {
                    _signingUp = false;
                    OnSignedUp?.Invoke(authenticationData);
                }, (exc) =>
                {
                    _signingUp = false;
                    OnFailedToSignUp?.Invoke(exc);
                });
        }

        private bool _signingInAsGoogle;

        private void SignInGoogle()
        {
            if(_signingInAsGoogle) return;
            _signingInAsGoogle = _authenticatorGoogle != null;
            OnSigningIn?.Invoke();
            _authenticatorGoogle?.GoogleSignIn(
                (authenticationData) =>
                {
                    _signingInAsGoogle = false;
                    OnSignedIn?.Invoke(authenticationData);
                }, (exc) =>
                {
                    _signingInAsGoogle = false;
                    OnFailedToSignIn?.Invoke(exc);
                });
        }

        private bool _signingInAsFacebook;
        private void SignInFacebook()
        {
            if(_signingInAsFacebook) return;
            _signingInAsFacebook = _authenticatorFacebook != null;
            OnSigningIn?.Invoke();
            _authenticatorFacebook?.FacebookSignIn(
                (authenticationData) =>
                {
                    _signingInAsFacebook = false;
                    OnSignedIn?.Invoke(authenticationData);
                }, (exc) =>
                {
                    _signingInAsFacebook = false;
                    OnFailedToSignIn?.Invoke(exc);
                });
        }

        [ContextMenu("SignOut")]
        public void SignOut()
        {
            if (!SignedIn)
            {
                OnFailedToSignOut?.Invoke("No User Logged In.");
                return;
            }
            OnSigningOut?.Invoke();
            _authenticator?.SignOut();
            OnSignedOut?.Invoke();
        }
        #endregion
        
        #region UpdateAuthenticatedData

        private bool _updatingDisplayName;
        public void UpdateUsersDisplayName(string displayName)
        {
            if(_updatingDisplayName) return;
            _updatingDisplayName = _authenticationData != null;
            OnUpdatingInfo?.Invoke();
            var authenticatedData = _authenticatedData; 
            _authenticationData?.UpdateDisplayName(displayName, () =>
            {
                _authenticatedData = new AuthenticationData()
                {
                    UserId = authenticatedData.UserId,
                    DisplayName = displayName,
                    EmailAddress = authenticatedData.EmailAddress,
                    IsAnonymous = authenticatedData.IsAnonymous,
                    PhotoUrl = authenticatedData.PhotoUrl,
                    Provider = authenticatedData.Provider,
                    Providers = authenticatedData.Providers
                };
                
                _updatingDisplayName = false;
                OnInfoUpdated?.Invoke(_authenticatedData);
            }, (exc) =>
            {
                _updatingDisplayName = false;
                OnFailedToUpdateInfo?.Invoke(exc);
            });
        }

        private bool _updatingUsersImage;
        public void UpdateUsersImage(string profileUrl)
        {
            if(_updatingUsersImage) return;
            _updatingUsersImage = _authenticationData != null;
            OnUpdatingInfo?.Invoke();
            var authenticatedData = _authenticatedData; 
            _authenticationData?.UpdateProfileImage(profileUrl, () =>
            {
                _authenticatedData = new AuthenticationData()
                {
                    UserId = authenticatedData.UserId,
                    DisplayName = authenticatedData.DisplayName,
                    EmailAddress = authenticatedData.EmailAddress,
                    IsAnonymous = authenticatedData.IsAnonymous,
                    PhotoUrl = new Uri(profileUrl),
                    Provider = authenticatedData.Provider,
                    Providers = authenticatedData.Providers
                };
                
                _updatingUsersImage = false;
                OnInfoUpdated?.Invoke(_authenticatedData);
            }, (exc) =>
            {
                _updatingUsersImage = false;
                OnFailedToUpdateInfo?.Invoke(exc);
            });
        }

        private bool _updatingDisplayNameAndImage;
        public void UpdateUsersDisplayNameAndImage(string displayName, string imageUrl)
        {
            if(_updatingDisplayNameAndImage) return;
            _updatingDisplayNameAndImage = _authenticationData != null;
            OnUpdatingInfo?.Invoke();
            var authenticatedData = _authenticatedData; 
            _authenticationData?.UpdateAuthenticatedData(displayName, imageUrl, () =>
            {
                _authenticatedData = new AuthenticationData()
                {
                    UserId = authenticatedData.UserId,
                    DisplayName = displayName,
                    EmailAddress = authenticatedData.EmailAddress,
                    IsAnonymous = authenticatedData.IsAnonymous,
                    PhotoUrl = new Uri(imageUrl),
                    Provider = authenticatedData.Provider,
                    Providers = authenticatedData.Providers
                };
                _updatingDisplayNameAndImage = false;
                OnInfoUpdated?.Invoke(_authenticatedData);
            }, (exc) =>
            {
                _updatingDisplayNameAndImage = false;
                OnFailedToUpdateInfo?.Invoke(exc);
            });
        }
        
        #endregion
        
        #region Linking
        
        private bool _linkingToCustomAccount;
        public void LinkToCustomAccount(string email, string password)
        {
            if(_linkingToCustomAccount) return;
            _linkingToCustomAccount = _linker != null;
            OnLinkingAccount?.Invoke();
            _linker?.LinkAccount(email, password, (authData) =>
            {
                _linkingToCustomAccount = false;
                OnAccountLinked?.Invoke(authData);
            }, (exc) =>
            {
                _linkingToCustomAccount = false;
                OnAccountFailedToLink?.Invoke(exc);
            });
        }
        
        private bool _unlinkingToCustomAccount;
        public void UnlinkFromCustomAccount()
        {
            if(_unlinkingToCustomAccount) return;
            _unlinkingToCustomAccount = _linker != null;
            OnUnlinkingAccount?.Invoke();
            _linker?.UnlinkAccount((authData) =>
            {
                _unlinkingToCustomAccount = false;
                OnAccountUnlinked?.Invoke(authData);
            }, (exc) =>
            {
                _unlinkingToCustomAccount = false;
                OnAccountFailedToUnlink?.Invoke(exc);
            });
        }
        
        
        private bool _linkingToGoogle;
        public void LinkToGoogle()
        {
            if(_linkingToGoogle) return;
            _linkingToGoogle = _authenticatorGoogle != null;
            OnLinkingAccount?.Invoke();
            _authenticatorGoogle?.LinkToGoogle((authData) =>
            {
                _linkingToGoogle = false;
                OnAccountLinked?.Invoke(authData);
            }, (exc) =>
            {
                _linkingToGoogle = false;
                OnAccountFailedToLink?.Invoke(exc);
            });
        }
        
        private bool _unlinkingToGoogle;
        public void UnlinkFromGoogle()
        {
            if(_unlinkingToGoogle) return;
            _unlinkingToGoogle = _authenticatorGoogle != null;
            OnUnlinkingAccount?.Invoke();
            _authenticatorGoogle?.UnlinkGoogleAccount((authData) =>
            {
                _unlinkingToGoogle = false;
                OnAccountUnlinked?.Invoke(authData);
            }, (exc) =>
            {
                _unlinkingToGoogle = false;
                OnAccountFailedToUnlink?.Invoke(exc);
            });
        }
        
        private bool _linkingToFacebook;
        public void LinkToFacebook()
        {
            if (_linkingToFacebook) return;
            _linkingToFacebook = _authenticatorFacebook != null;
            OnLinkingAccount?.Invoke();
            _authenticatorFacebook?.LinkToFacebook((authData) =>
            {
                _linkingToFacebook = false;
                OnAccountLinked?.Invoke(authData);
            }, (exc) =>
            {
                _linkingToFacebook = false;
                OnAccountFailedToLink?.Invoke(exc);
            });
        }
        
        private bool _unlinkingToFacebook;
        public void UnlinkFromFacebook()
        {
            if (_unlinkingToFacebook) return;
            _unlinkingToFacebook = _authenticatorFacebook != null;
            OnUnlinkingAccount?.Invoke();
            _authenticatorFacebook?.UnLinkFacebookAccount((authData) =>
            {
                _unlinkingToFacebook = false;
                OnAccountUnlinked?.Invoke(authData);
            }, (exc) =>
            {
                _unlinkingToFacebook = false;
                OnAccountFailedToUnlink?.Invoke(exc);
            });
        }
        
        #endregion

        private void UpdateAuthenticatedData(AuthenticationData authentication)
        {
            _authenticatedData = new AuthenticationData()
            {
                UserId = authentication.UserId,
                DisplayName = authentication.DisplayName,
                EmailAddress = authentication.EmailAddress,
                IsAnonymous = authentication.IsAnonymous,
                PhotoUrl = authentication.PhotoUrl,
                Provider = authentication.Provider,
                Providers = authentication.Providers
            };
            OnAuthenticatedDataChanged?.Invoke(_authenticatedData);
        }
        
        private void OnDisable()
        {
            _authenticationData.OnSignedIn -= LoggedIn;
            _authenticationData.OnSignedOut -= FailedToLogInOrHasLoggedOut;
            
            OnAccountLinked -= UpdateAuthenticatedData;
            OnAccountUnlinked -= UpdateAuthenticatedData;
        }
    }

    public struct AuthenticationData
    {
        public bool IsAnonymous;
        public string UserId;
        public string DisplayName;
        public string EmailAddress;
        public Uri PhotoUrl;
        public string Provider;
        public List<string> Providers;
    }
}