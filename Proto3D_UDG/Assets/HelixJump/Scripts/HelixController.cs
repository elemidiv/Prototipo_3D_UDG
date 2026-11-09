using UnityEngine;
using UnityEngine.InputSystem;

namespace HelixJump
{
    /// <summary>Capítulo 3: arrastrar horizontalmente gira la torre, no la bola.</summary>
    [DisallowMultipleComponent]
    public sealed class HelixController : MonoBehaviour
    {
        [SerializeField, Min(0.01f)] float degreesPerPixel = 0.35f;
        bool dragging;
        Vector2 lastPosition;

        void Update()
        {
            var touch = Touchscreen.current;
            if (touch != null && touch.primaryTouch.press.isPressed)
                ProcessPointer(touch.primaryTouch.position.ReadValue(), true);
            else if (Mouse.current != null)
                ProcessPointer(Mouse.current.position.ReadValue(), Mouse.current.leftButton.isPressed);
            else
                ProcessPointer(Vector2.zero, false);
        }

        public void ProcessPointer(Vector2 position, bool pressed)
        {
            if (!pressed) { dragging = false; return; }
            if (!dragging) { lastPosition = position; dragging = true; return; }
            float distance = lastPosition.x - position.x;
            transform.Rotate(Vector3.up, distance * degreesPerPixel, Space.Self);
            lastPosition = position;
        }

        void OnDisable() => dragging = false;
    }
}
