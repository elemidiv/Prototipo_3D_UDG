using UnityEngine;

namespace HelixJump
{
    /// <summary>Rebote del capítulo 2: impulso único por contacto, sin energía acumulada.</summary>
    [RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
    public sealed class BallController : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] float impulseForce = 3f;
        [SerializeField, Min(0f)] float collisionCooldown = 0.2f;
        Rigidbody body;
        float nextBounceTime = float.NegativeInfinity;
        public int BounceCount { get; private set; }
        public float ImpulseForce => impulseForce;

        void Awake()
        {
            body = GetComponent<Rigidbody>();
            body.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ |
                               RigidbodyConstraints.FreezeRotation;
            body.interpolation = RigidbodyInterpolation.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        void OnCollisionEnter(Collision collision)
        {
            // Solo las superficies bajo la bola producen un rebote; no las paredes laterales.
            foreach (var contact in collision.contacts)
                if (contact.normal.y > 0.5f)
                {
                    TryBounceAt(Time.time);
                    break;
                }
        }

        public bool TryBounceAt(float time)
        {
            if (time < nextBounceTime) return false;
            if (body == null) body = GetComponent<Rigidbody>();
            body.linearVelocity = Vector3.zero;
            body.AddForce(Vector3.up * impulseForce, ForceMode.Impulse);
            nextBounceTime = time + collisionCooldown;
            BounceCount++;
            return true;
        }
    }
}
