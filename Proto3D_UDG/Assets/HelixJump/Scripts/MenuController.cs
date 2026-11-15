using UnityEngine;
using UnityEngine.SceneManagement;
namespace HelixJump
{
    public sealed class MenuController : MonoBehaviour
    {
        public void Play() => SceneManager.LoadScene("HelixJump", LoadSceneMode.Single);
        public void Quit()
        {
#if UNITY_EDITOR
            Debug.Log("Salir del juego (Application.Quit solo cierra la compilación).");
#else
            Application.Quit();
#endif
        }
    }
}
