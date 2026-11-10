using UnityEngine;

namespace HelixJump
{
    /// <summary>Capítulos 3 y 4: puntuación de la partida y mejor resultado local.</summary>
    public sealed class GameManager : MonoBehaviour
    {
        public const string HighScoreKey = "HelixJump.HighScore";
        public static GameManager Instance { get; private set; }
        public int CurrentScore { get; private set; }
        public int BestScore { get; private set; }
        public int CurrentLevel { get; private set; }
        public bool GoalReached { get; private set; }

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            CurrentScore = 0;
            BestScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        }

        public void AddScore(int score)
        {
            if (score <= 0) return;
            CurrentScore += score;
            if (CurrentScore <= BestScore) return;
            BestScore = CurrentScore;
            PlayerPrefs.SetInt(HighScoreKey, BestScore);
            PlayerPrefs.Save();
        }

        public void NextLevel()
        {
            // El capítulo 4 solo deja el aviso; el cambio real se enseña después.
            if (GoalReached) return;
            GoalReached = true;
            Debug.Log("Pasamos de nivel");
        }

        public void RestartLevel()
        {
            CurrentScore = 0;
            GoalReached = false;
            foreach (var point in FindObjectsByType<PassScorePoint>(FindObjectsInactive.Include, FindObjectsSortMode.None)) point.Rearm();
            foreach (var ball in FindObjectsByType<BallController>(FindObjectsSortMode.None)) ball.ResetBall();
            foreach (var ui in FindObjectsByType<UIManager>(FindObjectsSortMode.None)) ui.RefreshNow();
        }

        void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }
    }
}
