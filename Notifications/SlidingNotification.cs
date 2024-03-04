using System;
using System.Collections;
using _Project.Scripts.HelperClasses;
using UnityEngine;

namespace _Project.Scripts.Notifications
{
    public abstract class SlidingNotification : LobbyNotification
    {
        [SerializeField] protected Vector2 _offset;
        [SerializeField] protected SlidingSide _slidingSide;
        
        protected NotificationData NData;
        protected RectTransform RectTransform;
        
        protected Coroutine ShowSlideNotificationCo;
        protected Coroutine HideSlideNotificationCo;
        
        public abstract IEnumerator ShowNotification();
        public abstract IEnumerator HideNotification();

        protected virtual IEnumerator Start()
        {
            yield return null;
            RectTransform = GetComponent<RectTransform>();
            RectTransform.SetPivot(GetPivot(_slidingSide));
            RectTransform.SetAnchor(GetAnchorPresets(_slidingSide), GetOffset(_slidingSide));
            RectTransform.anchoredPosition = GetStartPosition();
        }
        
        protected IEnumerator SlidingNotificationTo(float value, float maxTime)
        {
            var target = new Vector2(
                    _slidingSide is SlidingSide.Left or SlidingSide.Right
                        ? value
                        : RectTransform.anchoredPosition.x, 
                    _slidingSide is SlidingSide.Top or SlidingSide.Bottom
                        ? value
                        : RectTransform.anchoredPosition.y);
            var targetToReach = _slidingSide is SlidingSide.Left or SlidingSide.Right
                ? target.x
                : target.y;
            var time = 0f;
            while (time < maxTime)
            {
                var currentPosition = RectTransform.anchoredPosition;
                var axisToMatch = _slidingSide is SlidingSide.Left or SlidingSide.Right
                    ? currentPosition.x
                    : currentPosition.y;

                if (Mathf.Approximately(targetToReach, axisToMatch))
                {
                    RectTransform.anchoredPosition = target;
                    yield break;
                }

                var newPosition = Vector2.Lerp(currentPosition, target,
                    time / maxTime);
                RectTransform.anchoredPosition = newPosition;
                time += Time.deltaTime;
                yield return null;
            }
        }
        
        protected AnchorPresets GetAnchorPresets(SlidingSide slidingSide)
        {
            return slidingSide switch
            {
                SlidingSide.Top => AnchorPresets.TopCenter,
                SlidingSide.Bottom => AnchorPresets.BottomCenter,
                SlidingSide.Left => AnchorPresets.MiddleLeft,
                SlidingSide.Right => AnchorPresets.MiddleRight,
                _ => throw new ArgumentOutOfRangeException(nameof(slidingSide), slidingSide, null)
            };
        }
        
        protected PivotPresets GetPivot(SlidingSide slidingSide)
        {
            switch (_slidingSide)
            {
                case SlidingSide.Top:
                    return PivotPresets.TopCenter;
                case SlidingSide.Bottom:
                    return PivotPresets.BottomCenter;
                case SlidingSide.Left:
                    return PivotPresets.MiddleLeft;
                case SlidingSide.Right:
                    return PivotPresets.MiddleRight;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        protected Vector2 GetOffset(SlidingSide slidingSide)
        {
            var rect = RectTransform.rect;
            return slidingSide switch
            {
                SlidingSide.Top => new Vector2(_offset.x, rect.height),
                SlidingSide.Bottom => new Vector2(_offset.x, -rect.height),
                SlidingSide.Left => new Vector2(-rect.width, _offset.y),
                SlidingSide.Right => new Vector2(rect.width, _offset.y),
                _ => throw new ArgumentOutOfRangeException(nameof(slidingSide), slidingSide, null)
            };
        }
        
        protected float GetResetPosition()
        {
            var rect = RectTransform.rect;
            return _slidingSide switch
            {
                SlidingSide.Top => rect.height,
                SlidingSide.Bottom => -rect.height,
                SlidingSide.Left => -rect.width,
                SlidingSide.Right => rect.width,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        protected Vector2 GetStartPosition()
        {
            var rect = RectTransform.rect;
            return _slidingSide switch
            {
                SlidingSide.Top => new Vector2(_offset.x, rect.height),
                SlidingSide.Bottom => new Vector2(_offset.x, -rect.height),
                SlidingSide.Left => new Vector2(-rect.width,_offset.y),
                SlidingSide.Right => new Vector2(rect.width,_offset.y),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
    
    public enum SlidingSide
    {
        Top,
        Bottom,
        Left,
        Right
    }
}