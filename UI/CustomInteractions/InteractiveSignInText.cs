using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Project.Scripts.UI.CustomInteractions
{
    public class InteractiveSignInText : InteractiveText
    {
        public static event Action DoShowSignIn;
        
        private void OnEnable() => RegisterEvents(ShowSignInButton);

        public void ShowSignInButton() => DoShowSignIn?.Invoke();

        private void OnDisable() => UnregisterEvents(ShowSignInButton);
    }
}