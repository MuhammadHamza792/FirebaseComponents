using System;
using _Project.Scripts.Analytics;
using _Project.Scripts.Authentication.Firebase;
using _Project.Scripts.Database.Firebase;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

namespace _Project.Scripts.ThirdParty
{
    public class FirebaseInitializer : MonoBehaviour
    {
        public static FirebaseInitializer Instance { private set; get; }

        public FirebaseApp FirebaseApp { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        private void Start()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("Firebase dependency checker was cancelled");
                    return;
                }
                
                if (task.IsFaulted)
                {
                    Debug.LogError("Encountered an error while resolving firebase dependencies : " + task.Exception);
                    return;
                }
                
                Debug.Log("Firebase dependency Resolved!");

                FirebaseApp = FirebaseApp.DefaultInstance;

                if (FirebaseAuthenticatorHandler.Instance != null)
                {
                    Debug.Log("Initializing firebase authentication.");
                    FirebaseAuthenticatorHandler.Instance.InitializeFirebaseAuthentication();
                }

                if (FirebaseDatabaseHandler.Instance != null)
                {
                    Debug.Log("Initializing firebase realtime database.");
                    FirebaseDatabaseHandler.Instance.InitializeDatabase();
                }

                if (FirebaseAnalyticsHandler.Instance != null)
                {
                    Debug.Log("Initializing firebase Analytics.");
                    FirebaseAnalyticsHandler.Instance.InitializeAnalytics();
                }

                /*if (FirebaseAuthenticatorHandler.Instance != null)
                {
                    FirebaseAuthenticatorHandler.Instance.DelegateAuthState();
                }*/
            });
        }
    }
}