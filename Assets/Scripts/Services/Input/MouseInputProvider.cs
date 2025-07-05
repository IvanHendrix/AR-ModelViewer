using UnityEngine;
using UnityEngine.InputSystem;

namespace Services.Input
{
    public class MouseInputProvider : IInputProvider
     {
         public bool TryGetTap(out Vector2 position)
         {
             position = default;
     
             if (Mouse.current.leftButton.wasPressedThisFrame)
             {
                 position = Mouse.current.position.ReadValue();
                 return true;
             }
     
             return false;
         }
     
         public bool TryGetSwipeDelta(out Vector2 delta)
         {
             delta = default;
     
             if (Mouse.current.leftButton.isPressed)
             {
                 delta = Mouse.current.delta.ReadValue();
                 return true;
             }
     
             return false;
         }
     
         public bool IsPinching(out float pinchDelta)
         {
             pinchDelta = 0f;
             return false;
         }
     }
}