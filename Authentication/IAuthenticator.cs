using System;
using System.Collections.Generic;
using Firebase.Auth;

namespace _Project.Scripts.Authentication
{
    public interface IAuthenticationData
    {
        public event Action OnSignedIn;
        public event Action OnSignedOut;
        
        public bool IsAnonymous { get; }
        public bool SignedIn { get; }
        public string UserId { get; }
        public string DisplayName { get; }
        public string EmailAddress { get; }
        public Uri PhotoUrl { get; }
        public string Provider { get; }
        public List<string> Providers { get; }

        public void UpdateDisplayName(string displayName, Action onCompleted = null, Action<string> onFailed = null);
        public void UpdateProfileImage(string photoUrl, Action onCompleted = null, Action<string> onFailed = null);
        public void UpdateAuthenticatedData(string displayName, string photoUrl, Action onCompleted = null, Action<string> onFailed = null);
    }

    public interface IAuthenticator
    {
        public void SignUp(string email, string password, Action<AuthenticationData> onComplete = null, Action<string> onFailure = null); 
        public void SignIn(string email, string password, Action<AuthenticationData> onComplete = null, Action<string> onFailure = null);
        public void SignInGuest(Action<AuthenticationData> onComplete = null, Action<string> onFailure = null);
        public void SignOut();
    }
    
    public interface ILinker
    {
        public void LinkAccount(string email, string pass, Action<AuthenticationData> onComplete = null, Action<string> onFailure = null);
        public void UnlinkAccount(Action<AuthenticationData> onComplete = null, Action<string> onFailure = null);
    }
    
    public interface ISignInGoogle
    {
        public void GoogleSignIn(Action<AuthenticationData> onComplete = null, Action<string> onFailure = null);
        public void LinkToGoogle(Action<AuthenticationData> onComplete = null, Action<string> onFailure = null);
        public void UnlinkGoogleAccount(Action<AuthenticationData> onComplete = null, Action<string> onFailure = null);
    }
    
    public interface ISignInFacebook
    {
        public void FacebookSignIn(Action<AuthenticationData> onComplete = null, Action<string> onFailure = null);
        public void LinkToFacebook(Action<AuthenticationData> onComplete = null, Action<string> onFailure = null);
        public void UnLinkFacebookAccount(Action<AuthenticationData> onComplete = null, Action<string> onFailure = null);
    }
}