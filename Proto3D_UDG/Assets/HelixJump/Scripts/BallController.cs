using UnityEngine;

namespace HelixJump
{
    /// <summary>Rebote del capítulo 2: impulso único por contacto, sin energía acumulada.</summary>
    [RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
    public sealed class BallController : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] float impulseForce = 3f;
        [SerializeField, Min(0f)] float collisionCooldown = 0.2f;
        [SerializeField, Min(1)] int perfectPassThreshold = 3;
        [SerializeField, Min(0.1f)] float superSpeedImpulse = 8f;
        public int PerfectPassCount { get; private set; }
        public bool SuperSpeedActive { get; private set; }
        public int SuperActivationCount { get; private set; }
        public ChristmasBallEffects Effects => GetComponent<ChristmasBallEffects>();
        Rigidbody body;
        Vector3 initialPosition;
        bool initialized;
        float nextBounceTime = float.NegativeInfinity;
        public int BounceCount { get; private set; }
        public float ImpulseForce => impulseForce;

        void Awake()
        {
            initialPosition = transform.position;
            initialized = true;
            body = GetComponent<Rigidbody>();
            body.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ |
                               RigidbodyConstraints.FreezeRotation;
            body.interpolation = RigidbodyInterpolation.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        void OnCollisionEnter(Collision collision)
        {
            bool supported = false;
            foreach (var contact in collision.contacts) if (contact.normal.y > 0.5f) { supported = true; break; }
            HandlePlatformContact(collision.collider, supported, Time.time);
        }

        // Returns true only for an accepted bounce; cooldown suppresses adjacent-sector duplicates.
        public bool HandlePlatformContact(Collider collider, bool supported, float time)
        {
            if (collider == null) return false;
            bool goal = collider.GetComponentInParent<GoalController>() != null;
            if (goal) { ClearCombo(); return false; } // GoalController realiza la transición.
            var platform = collider.GetComponentInParent<BreakablePlatform>();
            // Ignorar contactos ya encolados del mismo anillo que acabamos de romper.
            if (platform != null && platform.Breaking) return false;
            if (SuperSpeedActive && platform != null && time >= nextBounceTime)
            {
                if (platform.TryBreak()) return TryBounceAt(time);
            }
            if (collider.GetComponentInParent<DeathPart>() != null)
            {
                ClearCombo();
                if (GameManager.Instance != null) GameManager.Instance.RestartLevel(); else ResetBall();
                return false;
            }
            if (time < nextBounceTime) return false;
            if (!supported) { ClearCombo(); return false; }
            return TryBounceAt(time);
        }

        public void RegisterPerfectPass()
        {
            PerfectPassCount++;
            if (PerfectPassCount < perfectPassThreshold || SuperSpeedActive) return;
            SuperSpeedActive = true;
            SuperActivationCount++;
            if (body == null) body = GetComponent<Rigidbody>();
            // El impulso se suma a la caída existente; no reducir una caída ya rápida.
            body.AddForce(Vector3.down * superSpeedImpulse, ForceMode.Impulse);
            if (Effects != null) Effects.ActivateSuper();
        }
        void ClearCombo()
        {
            PerfectPassCount = 0;
            SuperSpeedActive = false;
            if (Effects != null) Effects.EndSuper();
        }

        public void ResetBall()
        {
            if (!initialized) { initialPosition = transform.position; initialized = true; }
            if (body == null) body = GetComponent<Rigidbody>();
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            body.position = initialPosition;
            transform.position = initialPosition;
            foreach (var trail in GetComponentsInChildren<TrailRenderer>(true)) trail.Clear();
            nextBounceTime = float.NegativeInfinity;
            BounceCount = 0;
            SuperActivationCount = 0;
            ClearCombo();
            if (Effects != null) Effects.ClearAll();
            foreach (var ui in FindObjectsByType<UIManager>(FindObjectsSortMode.None)) ui.ResetProgress();
        }

        public bool TryBounceAt(float time)
        {
            if (time < nextBounceTime) return false;
            if (body == null) body = GetComponent<Rigidbody>();
            body.linearVelocity = Vector3.zero;
            body.AddForce(Vector3.up * impulseForce, ForceMode.Impulse);
            nextBounceTime = time + collisionCooldown;
            BounceCount++;
            ClearCombo();
            if (Effects != null) Effects.Bounce();
            return true;
        }
    }
}
