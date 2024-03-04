using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Authentication;
using _Project.Scripts.Database;
using _Project.Scripts.Notifications;
using Firebase.Database;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace _Project.Scripts.UI
{
    public class ProfilingUI : MonoBehaviour, INotifier
    {
        [SerializeField] private float _maxImageLoadingTime;
        [SerializeField] private Image _profileImage;
        [SerializeField] private TextMeshProUGUI _imageLoadingText;
        [SerializeField] private TextMeshProUGUI _displayName;
        [SerializeField] private TextMeshProUGUI _emailAddress;
        [SerializeField] private TextMeshProUGUI _providerName;
        [SerializeField] private TextMeshProUGUI _coinsText;
        
        [SerializeField] private List<TextMeshProUGUI> _providerNames;
        [SerializeField] private Button _addCoins;
        [SerializeField] private Button _minusCoins;
        [SerializeField] private Button _signOut;

        public static event Action DoSignOut;

        private Coroutine _imageCo;
        private Coroutine _timeOutCo;
        private bool _imageRequestCompleted;
        private bool _timedOut;
        private int _coins;
        private AuthenticationData _authData;

        private int Coins
        {
            set
            {
                _coins = value;
                _coinsText.SetText(_coins.ToString());
            }
            get => _coins;
        }

        private void Awake()
        {
            Authenticator.OnSigningOut += SigningOut;
            Authenticator.OnSignedOut += SignedOut;
            Authenticator.OnFailedToSignOut += FailedToSignOut;
            
            Authenticator.OnLinkingAccount += LinkingAccount;
            Authenticator.OnAccountLinked += AccountLinked;
            Authenticator.OnAccountFailedToLink += AccountFailedToLink;
            
            Authenticator.OnUnlinkingAccount += UnlinkingAccount;
            Authenticator.OnAccountUnlinked += AccountUnlinked;
            Authenticator.OnAccountFailedToUnlink += AccountFailedToUnlink;
        }
        
        
        private void OnEnable()
        {
            _signOut.onClick.AddListener(SignOut);
            _addCoins.onClick.AddListener(AddCoins);
            _minusCoins.onClick.AddListener(MinusCoins);
        }
        
        private void LinkingAccount() =>
            NotificationHelper.SendNotification(NotificationType.Progress, "Link Account", "Linking Account", this, NotifyCallType.Open);

        private void AccountLinked(AuthenticationData obj) => 
            NotificationHelper.SendNotification(NotificationType.Progress, "Link Account", "Account Linked", this, NotifyCallType.Close);

        private void AccountFailedToLink(string exc)
        {
            NotificationHelper.SendNotification(NotificationType.Progress, "Link Account", "Failed To Link Account", this, NotifyCallType.Close);
            NotificationHelper.SendNotification(NotificationType.Info, "Link Account", exc, this, NotifyCallType.Open);
        }

        private void UnlinkingAccount() =>
            NotificationHelper.SendNotification(NotificationType.Progress, "Unlink Account", "Unlinking Account", this,
                NotifyCallType.Open);

        private void AccountUnlinked(AuthenticationData obj) => 
            NotificationHelper.SendNotification(NotificationType.Progress, "Unlink Account", "Account Unlinked", this, NotifyCallType.Close);

        private void AccountFailedToUnlink(string exc)
        {
            NotificationHelper.SendNotification(NotificationType.Progress, "Unlink Account", "Failed To Unlink Account", this, NotifyCallType.Close);
            NotificationHelper.SendNotification(NotificationType.Info, "Unlink Account", exc, this, NotifyCallType.Open);
        }

        
        private void SigningOut() => 
            NotificationHelper.SendNotification(NotificationType.Progress, "Signing out", "Signing Out.", this, NotifyCallType.Open);

        private void SignedOut() => 
            NotificationHelper.SendNotification(NotificationType.Progress, "Signing out", "Signed Out.", this, NotifyCallType.Close);

        private void FailedToSignOut(string exc)
        {
            NotificationHelper.SendNotification(NotificationType.Progress, "Signing out", "Failed Signed Out.", this, NotifyCallType.Close);
            NotificationHelper.SendNotification(NotificationType.Info, "Signing out", exc, this, NotifyCallType.Open);
        }

        public void SetProfileData(AuthenticationData authenticationData)
        {
            _authData = authenticationData;
            
            _displayName.SetText(authenticationData.DisplayName);
            _emailAddress.SetText(authenticationData.EmailAddress);
            _providerName.SetText(authenticationData.Provider);

            
            for (int i = 0; i < _providerNames.Count; i++)
            {
                var providerName = _providerNames[i];
                providerName.gameObject.SetActive(false);
            }

            if (authenticationData.Providers != null)
            {
                for (int i = 0; i < authenticationData.Providers.Count; i++)
                {
                    var provider = authenticationData.Providers[i];
                    if (i >= _providerNames.Count) continue;
                    _providerNames[i].SetText($"{provider}");
                    _providerNames[i].gameObject.SetActive(true);
                }
            }
            
            if(_imageCo != null) StopCoroutine(_imageCo);
            _imageCo = StartCoroutine(LoadingImage(authenticationData.PhotoUrl));
            if (_timeOutCo != null) StopCoroutine(_timeOutCo);
            _timeOutCo = StartCoroutine(StartTimer());
        }
        
        private void MinusCoins()
        {
            Coins--;
            DatabaseHandler.Instance.UpdateData(_authData.UserId, "Coins", Coins.ToString(), "users", () =>
            {
                Debug.Log("Coins Updated");
            },()=>
            {
                Debug.Log("Coins Failed To Update");
            });
        }

        private void AddCoins()
        {
            Coins++;
            DatabaseHandler.Instance.UpdateData(_authData.UserId, "Coins", Coins.ToString(), "users", () =>
            {
                Debug.Log("Coins Updated");
            },()=>
            {
                Debug.Log("Coins Failed To Update");
            });
        }

        public void SetUserData(DataSnapshot data)
        {
            Coins = int.Parse(data.Child("Coins").Value.ToString());
        }

        private IEnumerator StartTimer()
        {
            var time = 0f;
            while (time < _maxImageLoadingTime)
            {
                if(_imageRequestCompleted) yield break;
                time += Time.deltaTime;
                yield return null;
            }
            
            if(_imageCo != null) StopCoroutine(_imageCo);
            SetImageLoadingText("Image timed out.");
        }

        private void SetImageLoadingText(string text)
        {
            _imageLoadingText.SetText(text);
            _imageLoadingText.gameObject.SetActive(true);
        }

        private IEnumerator LoadingImage(Uri imageUrl)
        {
            _imageRequestCompleted = false;
            
            if (imageUrl == default)
            {
                SetImageLoadingText("Image Not Available");
                _imageRequestCompleted = true;
                _profileImage.sprite = null;
                yield break;
            }
            
            SetImageLoadingText("Loading Image ...");

            var www = UnityWebRequestTexture.GetTexture(imageUrl);
            
            yield return www.SendWebRequest();

            _imageRequestCompleted = true;
            
            if (www.result != UnityWebRequest.Result.Success)
            {
                SetImageLoadingText("Image failed to load.");
                Debug.Log(www.error);
            }
            else
            {
                var imageTex = ((DownloadHandlerTexture)www.downloadHandler).texture;
                var profileSprite = Sprite.Create(imageTex, new Rect(0, 0, imageTex.width, imageTex.height), new Vector2(.5f, .5f));
                _imageLoadingText.gameObject.SetActive(false);
                _profileImage.sprite = profileSprite;
            }
        }
        private void SignOut() => DoSignOut?.Invoke();
        
        public void Notify(string notifyData = null, bool accepted = false, bool rejected = false)
        {
            throw new NotImplementedException();
        }
        
        private void OnDisable()
        {
            _signOut.onClick.RemoveListener(SignOut);
            _addCoins.onClick.RemoveListener(AddCoins);
            _minusCoins.onClick.RemoveListener(MinusCoins);
        }

        private void OnDestroy()
        {
            Authenticator.OnSigningOut -= SigningOut;
            Authenticator.OnSignedOut -= SignedOut;
            Authenticator.OnFailedToSignOut -= FailedToSignOut;
            
            Authenticator.OnLinkingAccount -= LinkingAccount;
            Authenticator.OnAccountLinked -= AccountLinked;
            Authenticator.OnAccountFailedToLink -= AccountFailedToLink;
            
            Authenticator.OnUnlinkingAccount -= UnlinkingAccount;
            Authenticator.OnAccountUnlinked -= AccountUnlinked;
            Authenticator.OnAccountFailedToUnlink -= AccountFailedToUnlink;
        }
    }
}
