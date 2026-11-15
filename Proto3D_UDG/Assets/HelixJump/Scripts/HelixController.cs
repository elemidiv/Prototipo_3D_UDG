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

        Quaternion initialRotation;
        bool rotationCaptured;

        void Awake() => CaptureRotation();
        void CaptureRotation()
        {
            if (rotationCaptured) return;
            initialRotation = transform.localRotation;
            rotationCaptured = true;
        }
        void Start() => LoadStage(0);

        public void ConfigureStage(Transform topReference, Transform goalReference, GameObject prefab, List<Stage> stageList, List<GameObject> spawned)
        { top = topReference; goal = goalReference; levelPrefab = prefab; stages = stageList; spawnedLevels = spawned ?? new List<GameObject>(); }

        // Capítulos 7 y 8: cargar el stage elegido, sin transición automática.
        public void LoadStage(int index)
        {
            if (stages == null || stages.Count == 0 || top == null || goal == null || levelPrefab == null) return;
            var stage = stages[Mathf.Clamp(index, 0, stages.Count - 1)];
            if (stage == null || stage.levels == null || stage.levels.Count == 0) return;
            if (top.localPosition.y <= goal.localPosition.y) return;
            CaptureRotation();
            transform.localRotation = initialRotation;
            dragging = false;
            var camera = Camera.main;
            if (camera != null) { camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = stage.backgroundColor; }
            var ball = FindFirstObjectByType<BallController>();
            if (ball != null && ball.TryGetComponent<Renderer>(out var renderer)) SetColor(renderer, stage.ballColor);
            // Las anclas son permanentes; nunca pertenecen a las plataformas generadas.
            foreach (var previous in spawnedLevels)
                if (previous != null && previous != top.gameObject && previous != goal.gameObject)
                {
                    previous.SetActive(false);
                    previous.transform.SetParent(null);
                    Remove(previous);
                }
            spawnedLevels.Clear();
            StageDistance = top.localPosition.y - goal.localPosition.y + 0.1f;
            float spacing = StageDistance / (stage.levels.Count + 1);
            for (int i = 0; i < stage.levels.Count; i++)
            {
                var level = Instantiate(levelPrefab, transform);
                level.name = "GeneratedLevel_" + (i + 1).ToString("00");
                level.transform.localPosition = new Vector3(0, top.localPosition.y - spacing * (i + 1), 0);
                level.transform.localRotation = Quaternion.identity;
                ConfigureParts(level, stage.levels[i] ?? new Level(), stage.helixColor);
                spawnedLevels.Add(level);
            }
        }

        static void ConfigureParts(GameObject level, Level settings, Color safeColor)
        {
            // El trigger es otro hijo: solo las piezas con MeshCollider son sectores.
            var sectors = new List<MeshRenderer>();
            foreach (var part in level.GetComponentsInChildren<MeshRenderer>(true))
                if (part.GetComponent<MeshCollider>() != null) sectors.Add(part);
            foreach (var part in sectors)
            {
                part.gameObject.SetActive(true);
                var oldDanger = part.GetComponent<DeathPart>();
                if (oldDanger != null) { oldDanger.enabled = false; Remove(oldDanger); }
                part.SetPropertyBlock(null);
                SetColor(part, safeColor);
            }
            // Barajar sin reposición evita repetir piezas y bucles imposibles.
            for (int i = sectors.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                var swap = sectors[i]; sectors[i] = sectors[j]; sectors[j] = swap;
            }
            int solid = Mathf.Clamp(settings.partCount, 1, Mathf.Max(1, sectors.Count - 1));
            for (int i = solid; i < sectors.Count; i++) sectors[i].gameObject.SetActive(false);
            int dangerous = Mathf.Clamp(settings.deathCount, 0, Mathf.Min(solid, sectors.Count));
            for (int i = 0; i < dangerous; i++) sectors[i].gameObject.AddComponent<DeathPart>();
            foreach (var point in level.GetComponentsInChildren<PassScorePoint>(true))
            { point.gameObject.SetActive(true); point.Rearm(); }
        }

        static void SetColor(Renderer renderer, Color color)
        {
            var properties = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(properties);
            properties.SetColor("_BaseColor", color);
            properties.SetColor("_Color", color);
            renderer.SetPropertyBlock(properties);
        }
        static void Remove(Object target)
        {
            if (Application.isPlaying) Destroy(target);
            else DestroyImmediate(target);
        }

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
