using System;
using _Project.Scripts.Authentication;
using _Project.Scripts.Notifications;
using _Project.Scripts.UI.CustomInteractions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI
{
    public class AuthenticationUI : MonoBehaviour, INotifier
    {
        [Header("Input Fields")]
        [SerializeField] private TMP_InputField _emailField;
        [SerializeField] private TMP_InputField _passwordField;

        [Header("Buttons")] 
        [SerializeField] private Button _signInBtn;
        [SerializeField] private Button _signUpBtn;
        [SerializeField] private Button _signInGuest;
        [SerializeField] private Button _signInGoogle;
        [SerializeField] private Button _signInFacebook;

        [Header("Container")] 
        [SerializeField] private GameObject _signInContainer;
        [SerializeField] private GameObject _signUpContainer;

        public static event Action<string, string> OnSignInWithCredentials;
        public static event Action<string, string> OnSignUpWithCredentials;
        public static event Action OnSignInGuest;
        public static event Action OnSignInGoogle;
        public static event Action OnSignInFacebook;

        private void Awake()
        {
            #region NotificationDelegates

            Authenticator.OnSigningIn += SigningIn;
            Authenticator.OnSignedIn += SignedIn;
            Authenticator.OnFailedToSignIn += FailedToSignIn;
            
            Authenticator.OnSigningUp += SigningUp;
            Authenticator.OnSignedUp += SignedUp;
            Authenticator.OnFailedToSignUp += FailedToSignUp;
            
            #endregion
        }

        private void OnEnable()
        {
            _signInBtn.onClick.AddListener(SignInWithCredentials);    
            _signUpBtn.onClick.AddListener(SignUpWithCredentials);    
            _signInGuest.onClick.AddListener(SignInGuest);    
            _signInGoogle.onClick.AddListener(SignInGoogle);    
            _signInFacebook.onClick.AddListener(SignInFacebook);
            InteractiveRegisterText.DoShowSignUp += EnableSignUpContainer;
            InteractiveSignInText.DoShowSignIn += EnableSignInContainer;
        }
        
        #region Notifications
        
        private void SigningUp() => 
            NotificationHelper.SendNotification(NotificationType.Progress, "Signing up", "Signing up ...", this, NotifyCallType.Open);

        private void SignedUp(AuthenticationData authenticationData) => 
            NotificationHelper.SendNotification(NotificationType.Progress, "Signing up", "Signed up", this, NotifyCallType.Close);

        private void FailedToSignUp(string exc)
        {
            NotificationHelper.SendNotification(NotificationType.Progress, "Signing up", "Failed to sign up", this, NotifyCallType.Close);
            NotificationHelper.SendNotification(NotificationType.Info, "Signing up", exc, this, NotifyCallType.Open);
        }

        private void SigningIn() => 
            NotificationHelper.SendNotification(NotificationType.Progress, "Signing in", "Signing in ...", this, NotifyCallType.Open);

        private void SignedIn(AuthenticationData authenticationData) => 
            NotificationHelper.SendNotification(NotificationType.Progress, "Signing in", "Signed in", this, NotifyCallType.Close);

        private void FailedToSignIn(string exc)
        {
            NotificationHelper.SendNotification(NotificationType.Progress, "Signing in", "Failed to sign in", this, NotifyCallType.Close);
            NotificationHelper.SendNotification(NotificationType.Info, "Signing in", exc, this, NotifyCallType.Open);
        }
        
        #endregion

        private void EnableSignUpContainer()
        {
            _signUpContainer.SetActive(true);
            _signInContainer.SetActive(false);
        }
        
        private void EnableSignInContainer()
        {
            _signInContainer.SetActive(true);
            _signUpContainer.SetActive(false);
        }
        
        private void SignInWithCredentials()
        {
            var email = _emailField.text;
            var pass = _passwordField.text;
            OnSignInWithCredentials?.Invoke(email, pass);
        }

        private void SignUpWithCredentials()
        {
            var email = _emailField.text;
            var pass = _passwordField.text;

            if (pass.Length < 8)
            {
                Debug.LogError("Password Should be at least 8 characters");
                return;
            }
            
            OnSignUpWithCredentials?.Invoke(email, pass);
        }

        private void SignInGuest() => OnSignInGuest?.Invoke();
        private void SignInGoogle() => OnSignInGoogle?.Invoke();
        private void SignInFacebook() => OnSignInFacebook?.Invoke();
        
        private void OnDisable()
        {
            _signInBtn.onClick.RemoveListener(SignInWithCredentials);    
            _signUpBtn.onClick.RemoveListener(SignUpWithCredentials);    
            _signInGuest.onClick.RemoveListener(SignInGuest);    
            _signInGoogle.onClick.RemoveListener(SignInGoogle);    
            _signInFacebook.onClick.RemoveListener(SignInFacebook);
            InteractiveRegisterText.DoShowSignUp -= EnableSignUpContainer;
            InteractiveSignInText.DoShowSignIn -= EnableSignInContainer;
        }

        private void OnDestroy()
        {
            #region NotificationDelegates

            Authenticator.OnSigningIn -= SigningIn;
            Authenticator.OnSignedIn -= SignedIn;
            Authenticator.OnFailedToSignIn -= FailedToSignIn;
            
            Authenticator.OnSigningUp -= SigningUp;
            Authenticator.OnSignedUp -= SignedUp;
            Authenticator.OnFailedToSignUp -= FailedToSignUp;
            
            #endregion
        }

        public void Notify(string notifyData = null, bool accepted = false, bool rejected = false)
        {
            
        }
    }
}
