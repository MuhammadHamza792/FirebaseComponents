using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace _Project.Scripts.UI.CustomInteractions
{
    public abstract class UIInteraction : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
    {
        [SerializeField] private bool _interactionOnPointerUp;

        public event Action OnInteracted;
        public UnityEvent OnUnityInteracted;
        
        public void OnPointerUp(PointerEventData eventData)
        {
            if(!_interactionOnPointerUp) return;
            DoAction();
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if(_interactionOnPointerUp) return;
            DoAction();
        }
        
        private void DoAction()
        {
            OnInteracted?.Invoke();
            OnUnityInteracted?.Invoke();
        }

        public void RegisterEvents(Action action) => OnInteracted += action;

        public void UnregisterEvents(Action action) => OnInteracted -= action;
    }
}
