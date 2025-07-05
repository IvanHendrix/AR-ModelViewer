using UnityEngine;

namespace Services.Input
{
    public interface IInputProvider
    {
        bool TryGetTap(out Vector2 position);
        bool TryGetSwipeDelta(out Vector2 delta);
        bool IsPinching(out float pinchAmount);
    }
}