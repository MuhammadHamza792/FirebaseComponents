using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Project.Scripts.UI.CustomInteractions
{
    public class InteractiveText : UIInteraction, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Color _colorOnHighlight;
        [SerializeField] private FontStyles _fontStylesOnHighlight;
        private Color _color;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            _text.fontStyle = _fontStylesOnHighlight;
            _color = _text.color;
            _text.color = _colorOnHighlight;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _text.fontStyle = FontStyles.Normal;
            _text.color = _color;
        }
    }
}