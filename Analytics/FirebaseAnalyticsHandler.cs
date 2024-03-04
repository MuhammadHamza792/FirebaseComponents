using System;
using System.Collections.Generic;
using Firebase.Analytics;
using UnityEngine;

namespace _Project.Scripts.Analytics
{
    public class FirebaseAnalyticsHandler : MonoBehaviour
    {
        private FirebaseAnalytics _firebaseAnalytics;
        
        public static FirebaseAnalyticsHandler Instance { private set; get; }

        public bool Initialized { private set; get; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }
        
        public void InitializeAnalytics()
        {
            Initialized = true;
            Debug.Log("Analytics initialized!");
        }

        public bool CheckIfAnalyticsAreInitialized() => Initialized;

        public void ScoreEvent(int score)
        {
            if (!Initialized)
            {
                Debug.LogError("Analytics not Initialized");
                return;
            }
            
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventPostScore, 
                new Parameter(FirebaseAnalytics.ParameterScore, score));
        }

        public void LoginEvent(string userId, string displayName, string email)
        {
            if (!Initialized)
            {
                Debug.LogError("Analytics not Initialized");
                return;
            }
            
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLogin, 
                new Parameter("userId", userId),
                new Parameter("displayName", displayName),
                new Parameter("email", email));
        }

        public void LevelEvent(AnalyticsLevelStates analyticsLevelState, int levelIndex)
        {
            if (!Initialized)
            {
                Debug.LogError("Analytics not Initialized");
                return;
            }
            
            FirebaseAnalytics.LogEvent(CheckLevelState(analyticsLevelState),
                new Parameter(FirebaseAnalytics.ParameterLevel, levelIndex));
        }
        
        public void LevelEvent(AnalyticsLevelStates analyticsLevelState, string levelName)
        {
            if (!Initialized)
            {
                Debug.LogError("Analytics not Initialized");
                return;
            }
            
            FirebaseAnalytics.LogEvent(CheckLevelState(analyticsLevelState),
                new Parameter(FirebaseAnalytics.ParameterLevelName, levelName));
        }
        
        public void LevelEvent(AnalyticsLevelStates analyticsLevelState, int levelIndex, string levelName)
        {
            if (!Initialized)
            {
                Debug.LogError("Analytics not Initialized");
                return;
            }

            FirebaseAnalytics.LogEvent(CheckLevelState(analyticsLevelState),
                new Parameter(FirebaseAnalytics.ParameterLevel, levelIndex),
                new Parameter(FirebaseAnalytics.ParameterLevelName, levelName));
        }

        public void TutorialEvent(AnalyticsTutorialStates analyticsTutorialState, string tutorialName)
        {
            if (!Initialized)
            {
                Debug.LogError("Analytics not Initialized");
                return;
            }
            
            FirebaseAnalytics.LogEvent(CheckTutorialState(analyticsTutorialState), 
                new Parameter(FirebaseAnalytics.ParameterLevelName, tutorialName));
        }

        public void AchievementUnlockedEvent(string achievementId)
        {
            if (!Initialized)
            {
                Debug.LogError("Analytics not Initialized");
                return;
            }
            
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventUnlockAchievement, 
                new Parameter(FirebaseAnalytics.ParameterAchievementId, achievementId));
        }

        public void VirtualCurrencyEarnedEvent(string currencyName, double currencyAmount)
        {
            if (!Initialized)
            {
                Debug.LogError("Analytics not Initialized");
                return;
            }
            
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventEarnVirtualCurrency,
                new Parameter(FirebaseAnalytics.ParameterCurrency, currencyName),
                new Parameter(FirebaseAnalytics.ParameterValue, currencyName));
        }
        
        public void PurchaseEvent(string currencyName, double currencyAmount, AnalyticsCurrencyType analyticsCurrencyType, string itemName, int itemAmount)
        {
            if (!Initialized)
            {
                Debug.LogError("Analytics not Initialized");
                return;
            }
            
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventPurchase, 
                new Parameter(FirebaseAnalytics.ParameterItemName, itemName),
                new Parameter(FirebaseAnalytics.ParameterQuantity, itemAmount),
                new Parameter(FirebaseAnalytics.ParameterCurrency, currencyName),
                new Parameter(FirebaseAnalytics.ParameterPrice, currencyAmount),
                new Parameter(FirebaseAnalytics.ParameterPaymentType, analyticsCurrencyType.ToString()));
        }
        
        private static string CheckLevelState(AnalyticsLevelStates analyticsLevelStates)
        {
            return analyticsLevelStates switch
            {
                AnalyticsLevelStates.Start => FirebaseAnalytics.EventLevelStart,
                AnalyticsLevelStates.Up => FirebaseAnalytics.EventLevelUp,
                AnalyticsLevelStates.End => FirebaseAnalytics.EventLevelEnd,
                _ => throw new ArgumentOutOfRangeException(nameof(analyticsLevelStates), analyticsLevelStates, null)
            };
        }
        
        private static string CheckTutorialState(AnalyticsTutorialStates analyticsTutorialStates)
        {
            return analyticsTutorialStates switch
            {
                AnalyticsTutorialStates.Start => FirebaseAnalytics.EventTutorialBegin,
                AnalyticsTutorialStates.End => FirebaseAnalytics.EventTutorialComplete,
                _ => throw new ArgumentOutOfRangeException(nameof(analyticsTutorialStates), analyticsTutorialStates, null)
            };
        }
    }

    public enum AnalyticsLevelStates
    {
        Start,
        Up,
        End
    }

    public enum AnalyticsTutorialStates
    {
        Start,
        End
    }

    public enum AnalyticsCurrencyType
    {
        VirtualCurrency,
        RealMoney
    }
}