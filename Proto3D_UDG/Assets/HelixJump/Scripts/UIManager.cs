using UnityEngine;
using UnityEngine.UI;

namespace HelixJump
{
    /// <summary>Capítulo 4: textos de la puntuación actual y el récord.</summary>
    public sealed class UIManager : MonoBehaviour
    {
        [SerializeField] Text currentScoreText;
        [SerializeField] Text bestScoreText;

        public void Configure(Text score, Text best)
        {
            currentScoreText = score;
            bestScoreText = best;
            RefreshNow();
        }

        void Update() => RefreshNow();

        public void RefreshNow()
        {
            var manager = GameManager.Instance;
            if (manager == null) return;
            if (currentScoreText != null) currentScoreText.text = "Score: " + manager.CurrentScore;
            if (bestScoreText != null) bestScoreText.text = "Best: " + manager.BestScore;
        }
    }
}
