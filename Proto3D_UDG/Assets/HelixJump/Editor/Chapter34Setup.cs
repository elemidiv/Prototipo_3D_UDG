using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace HelixJump.Editor
{
    public static class Chapter34Setup
    {
        [MenuItem("Helix Jump/Incorporar capítulos 3 y 4")]
        public static void Apply()
        {
            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (Application.isPlaying || scene.path != "Assets/HelixJump/Scenes/HelixJump.unity" || scene.isDirty)
                throw new InvalidOperationException("Abre y guarda la escena HelixJump fuera de Play antes de continuar.");
            if (UnityEngine.Object.FindFirstObjectByType<GameManager>() != null)
                throw new InvalidOperationException("La ampliación ya está aplicada; no se duplican componentes.");
            var helix = GameObject.Find("Helix");
            var canvas = UnityEngine.Object.FindFirstObjectByType<Canvas>();
            var title = canvas.transform.Find("GameTitle").GetComponent<Text>();
            var lower = helix.transform.Find("HelixLevel_02");
            var levelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/HelixJump/Prefabs/HelixLevel.prefab");
            const string goalPath = "Assets/HelixJump/Prefabs/HelixGoal.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(goalPath) != null)
                throw new InvalidOperationException("Ya existe HelixGoal; revisar antes de sobrescribir.");

            helix.AddComponent<HelixController>();
            new GameObject("GameManager").AddComponent<GameManager>();
            // La plataforma intermedia también necesita un hueco para llegar hasta la meta.
            lower.Find("HelixPart_08").gameObject.SetActive(false);
            lower.Find("HelixPart_09").gameObject.SetActive(false);

            var gold = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            gold.name = "Goal_Gold";
            gold.SetColor("_BaseColor", new Color32(216, 172, 66, 255));
            gold.SetFloat("_Metallic", 0.3f);
            gold.SetFloat("_Smoothness", 0.25f);
            AssetDatabase.CreateAsset(gold, "Assets/HelixJump/Materials/Goal_Gold.mat");
            var goal = (GameObject)PrefabUtility.InstantiatePrefab(levelPrefab);
            PrefabUtility.UnpackPrefabInstance(goal, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            goal.name = "HelixGoal";
            foreach (Transform part in goal.transform)
            {
                part.gameObject.SetActive(true);
                part.gameObject.AddComponent<GoalController>();
                part.GetComponent<Renderer>().sharedMaterial = gold;
            }
            var prefab = PrefabUtility.SaveAsPrefabAsset(goal, goalPath);
            UnityEngine.Object.DestroyImmediate(goal);
            goal = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            goal.transform.SetParent(helix.transform, false);
            goal.transform.localPosition = new Vector3(0, -3, 0);

            title.color = new Color32(245, 242, 226, 255);
            var titleOutline = title.GetComponent<Outline>();
            titleOutline.effectColor = new Color32(12, 40, 28, 255);
            titleOutline.effectDistance = new Vector2(3, -3);
            var score = Label(title, "CurrentScoreText", "Score: 0", 56, 0.79f, 0.85f);
            var best = Label(title, "BestScoreText", "Best: 0", 56, 0.79f, 0.85f);
            score.rectTransform.anchorMax = new Vector2(0.49f, 0.85f);
            best.rectTransform.anchorMin = new Vector2(0.51f, 0.79f);
            canvas.gameObject.AddComponent<UIManager>().Configure(score, best);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log("Helix Jump: capítulos 3 y 4 integrados. Arrastra para girar; la meta solo muestra el aviso.");
        }

        static Text Label(Text template, string name, string initialText, int size, float minY, float maxY)
        {
            var label = UnityEngine.Object.Instantiate(template, template.transform.parent);
            label.name = name;
            label.text = initialText;
            label.fontSize = size;
            label.fontStyle = FontStyle.Bold;
            label.raycastTarget = false;
            label.rectTransform.anchorMin = new Vector2(0.08f, minY);
            label.rectTransform.anchorMax = new Vector2(0.92f, maxY);
            label.rectTransform.offsetMin = label.rectTransform.offsetMax = Vector2.zero;
            return label;
        }
    }
}
