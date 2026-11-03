using UnityEngine;

namespace HelixJump
{
    /// <summary>Cámara del tutorial: sigue solamente Y y conserva X/Z.</summary>
    [DisallowMultipleComponent]
    public sealed class CameraFollow : MonoBehaviour
    {
        [SerializeField] Transform target;
        float verticalOffset;
        bool initialized;
        public Transform Target => target;
        public float VerticalOffset => verticalOffset;

        public void Configure(Transform newTarget)
        {
            target = newTarget;
            initialized = target != null;
            if (initialized) verticalOffset = transform.position.y - target.position.y;
        }

        void Start() => Configure(target);
        void LateUpdate() => FollowNow();

        public void FollowNow()
        {
            if (!initialized || target == null) return;
            var position = transform.position;
            position.y = target.position.y + verticalOffset;
            transform.position = position;
        }
    }
}
