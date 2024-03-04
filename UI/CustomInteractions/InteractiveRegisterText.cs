using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Project.Scripts.UI.CustomInteractions
{
    public class InteractiveRegisterText : InteractiveText
    {
        public static event Action DoShowSignUp;
        private void OnEnable() => RegisterEvents(ShowSignUpButton);

        public void ShowSignUpButton()
        {
            DoShowSignUp?.Invoke();
        }

        private void OnDisable() => UnregisterEvents(ShowSignUpButton);
    }
}
