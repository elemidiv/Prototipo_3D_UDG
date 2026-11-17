using UnityEngine;
namespace HelixJump
{
    [RequireComponent(typeof(BoxCollider))]
    public sealed class PassScorePoint : MonoBehaviour
    {
        public bool Passed { get; private set; }
        void Reset() => GetComponent<BoxCollider>().isTrigger = true;
        void OnTriggerEnter(Collider other) => TryPass(other);
        public bool TryPass(Collider other)
        {
            if (Passed || other == null || other.GetComponentInParent<BallController>() == null || GameManager.Instance == null) return false;
            Passed = true;
            GameManager.Instance.AddScore(1);
            other.GetComponentInParent<BallController>().RegisterPerfectPass();
            return true;
        }
        public void Rearm() => Passed = false;
    }
}
