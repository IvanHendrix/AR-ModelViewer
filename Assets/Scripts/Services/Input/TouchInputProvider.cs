using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Services.Input
{
    public class TouchInputProvider : IInputProvider
    {
        public TouchInputProvider()
        {
            EnhancedTouchSupport.Enable(); // важливо!
        }

        public bool TryGetTap(out Vector2 position)
        {
            position = default;

            foreach (var touch in Touch.activeTouches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    position = touch.screenPosition;
                    return true;
                }
            }

            return false;
        }

        public bool TryGetSwipeDelta(out Vector2 delta)
        {
            delta = default;

            foreach (var touch in Touch.activeTouches)
            {
                if (touch.phase == TouchPhase.Moved)
                {
                    delta = touch.delta;
                    return true;
                }
            }

            return false;
        }

        public bool IsPinching(out float pinchDelta)
        {
            pinchDelta = 0f;

            if (Touch.activeTouches.Count >= 2)
            {
                var t0 = Touch.activeTouches[0];
                var t1 = Touch.activeTouches[1];

                var prev = (t0.screenPosition - t0.delta) - (t1.screenPosition - t1.delta);
                var curr = t0.screenPosition - t1.screenPosition;

                pinchDelta = curr.magnitude - prev.magnitude;
                return true;
            }

            return false;
        }
    }
}