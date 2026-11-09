using UnityEngine;

namespace HelixJump
{
    /// <summary>Meta del capítulo 4. Anuncia la llegada; no carga otra escena.</summary>
    public sealed class GoalController : MonoBehaviour
    {
        void OnCollisionEnter(Collision collision) => TryReach(collision.collider);

        public bool TryReach(Collider other)
        {
            if (other == null || other.GetComponentInParent<BallController>() == null) return false;
            var manager = GameManager.Instance;
            if (manager == null || manager.GoalReached) return false;
            manager.NextLevel();
            return true;
        }
    }
}
