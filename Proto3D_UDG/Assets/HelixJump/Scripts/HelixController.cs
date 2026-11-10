using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HelixJump
{
    /// <summary>Capítulo 3: arrastrar horizontalmente gira la torre, no la bola.</summary>
    [DisallowMultipleComponent]
    public sealed class HelixController : MonoBehaviour
    {
        [SerializeField, Min(0.01f)] float degreesPerPixel = 0.35f;
        [SerializeField] Transform top;
        [SerializeField] Transform goal;
        [SerializeField] GameObject levelPrefab;
        [SerializeField] List<Stage> stages = new List<Stage>();
        [SerializeField] List<GameObject> spawnedLevels = new List<GameObject>();
        public float StageDistance { get; private set; }

        void Start()
        {
            if (top != null && goal != null) StageDistance = top.localPosition.y - goal.localPosition.y + 0.1f;
            // LoadStage(0); // La generación se implementa en un capítulo posterior.
        }

        public void ConfigureStage(Transform topReference, Transform goalReference, GameObject prefab, List<Stage> stageList, List<GameObject> spawned)
        { top = topReference; goal = goalReference; levelPrefab = prefab; stages = stageList; spawnedLevels = spawned; }

        public void LoadStage(int index) { }

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
