using System;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class HelixJump42Tests
{
    static Type Find(string name) => UnityEditor.TypeCache.GetTypesDerivedFrom<MonoBehaviour>()
        .FirstOrDefault(t => t.FullName == "HelixJump." + name);

    [Test]
    public void Scene_WiresDragScoreUIAndGoal_WithoutReplacingTheTitle()
    {
        var scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/HelixJump/Scenes/HelixJump.unity");
        var roots = scene.GetRootGameObjects();
        var components = roots.SelectMany(r => r.GetComponentsInChildren<MonoBehaviour>(true)).Where(c => c != null).ToArray();
        Assert.That(components.Count(c => c.GetType().FullName == "HelixJump.GameManager"), Is.EqualTo(1));
        Assert.That(components.Count(c => c.GetType().FullName == "HelixJump.HelixController"), Is.EqualTo(1));
        Assert.That(components.Count(c => c.GetType().FullName == "HelixJump.UIManager"), Is.EqualTo(1));
        Assert.That(components.Count(c => c.GetType().FullName == "HelixJump.GoalController"), Is.EqualTo(12));
        var labels = roots.SelectMany(r => r.GetComponentsInChildren<UnityEngine.UI.Text>()).ToArray();
        Assert.That(labels.Length, Is.EqualTo(3));
        Assert.That(labels.Single(t => t.name == "GameTitle").text, Is.EqualTo("Helix Jump"));
        Assert.That(labels.Single(t => t.name == "CurrentScoreText").text, Is.EqualTo("Score: 0"));
        Assert.That(labels.Single(t => t.name == "BestScoreText").text, Is.EqualTo("Best: 0"));
        Assert.That(UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/HelixJump/Prefabs/HelixGoal.prefab"), Is.Not.Null);
        var helix = roots.Single(r => r.name == "Helix");
        Assert.That(helix.transform.Find("HelixLevel_02"),Is.Null,"El capítulo 7 sustituye la plataforma intermedia manual por generación.");
        var top=helix.transform.Find("HelixLevel_01");
        Assert.That(Enumerable.Range(0,top.childCount).Any(i=>!top.GetChild(i).gameObject.activeSelf),Is.True,"El ancla superior conserva su hueco inicial.");
    }

    [Test]
    public void Goal_AcceptsOnlyBall_AndAnnouncesOnce_WithoutChangingLevel()
    {
        var goalType = Find("GoalController");
        Assert.That(goalType, Is.Not.Null, "Falta la meta del capítulo 4.");
        var managerType = Find("GameManager");
        var managerGo = new GameObject("GoalManagerTest");
        var goalGo = new GameObject("GoalTest");
        var ballGo = new GameObject("GoalBallTest");
        var otherGo = new GameObject("Other", typeof(BoxCollider));
        try
        {
            var manager = managerGo.AddComponent(managerType);
            managerType.GetMethod("Awake", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(manager, null);
            ballGo.AddComponent(Find("BallController"));
            var goal = goalGo.AddComponent(goalType);
            var reach = goalType.GetMethod("TryReach");
            Assert.That((bool)reach.Invoke(goal, new object[] { otherGo.GetComponent<Collider>() }), Is.False);
            UnityEngine.TestTools.LogAssert.Expect(LogType.Log, "Pasamos de nivel");
            Assert.That((bool)reach.Invoke(goal, new object[] { ballGo.GetComponent<Collider>() }), Is.True);
            Assert.That((bool)reach.Invoke(goal, new object[] { ballGo.GetComponent<Collider>() }), Is.False);
            Assert.That((int)managerType.GetProperty("CurrentLevel").GetValue(manager), Is.EqualTo(1));
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(goalGo); UnityEngine.Object.DestroyImmediate(ballGo);
            UnityEngine.Object.DestroyImmediate(otherGo); UnityEngine.Object.DestroyImmediate(managerGo);
        }
    }

    [Test]
    public void UI_ReflectsScoreAndBest_UsingTheExistingCanvasTextSystem()
    {
        var type = Find("UIManager");
        Assert.That(type, Is.Not.Null, "Falta UIManager del capítulo 4.");
        var managerType = Find("GameManager");
        var go = new GameObject("UIManagerTest");
        var managerGo = new GameObject("UIManagerScoreTest");
        var scoreGo = new GameObject("Score", typeof(RectTransform), typeof(UnityEngine.UI.Text));
        var bestGo = new GameObject("Best", typeof(RectTransform), typeof(UnityEngine.UI.Text));
        const string key = "HelixJump.HighScore";
        bool had = PlayerPrefs.HasKey(key);
        int previous = PlayerPrefs.GetInt(key, 0);
        try
        {
            PlayerPrefs.SetInt(key, 9);
            var manager = managerGo.AddComponent(managerType);
            managerType.GetMethod("Awake", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(manager, null);
            var ui = go.AddComponent(type);
            var score = scoreGo.GetComponent<UnityEngine.UI.Text>();
            var best = bestGo.GetComponent<UnityEngine.UI.Text>();
            type.GetMethod("Configure").Invoke(ui, new object[] { score, best });
            managerType.GetMethod("AddScore").Invoke(manager, new object[] { 1 });
            type.GetMethod("RefreshNow").Invoke(ui, null);
            Assert.That(score.text, Is.EqualTo("Score: 1"));
            Assert.That(best.text, Is.EqualTo("Best: 9"));
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(go); UnityEngine.Object.DestroyImmediate(managerGo);
            UnityEngine.Object.DestroyImmediate(scoreGo); UnityEngine.Object.DestroyImmediate(bestGo);
            if (had) PlayerPrefs.SetInt(key, previous); else PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
        }
    }

    [Test]
    public void Bounce_DoesNotScore_InChapterFive()
    {
        var managerType = Find("GameManager");
        var ballType = Find("BallController");
        Assert.That(managerType, Is.Not.Null);
        var managerGo = new GameObject("ScoreIntegrationManager");
        var ballGo = new GameObject("ScoreIntegrationBall");
        const string key = "HelixJump.HighScore";
        bool hadKey = PlayerPrefs.HasKey(key);
        int previous = PlayerPrefs.GetInt(key, 0);
        try
        {
            var manager = managerGo.AddComponent(managerType);
            managerType.GetMethod("Awake", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(manager, null);
            var ball = ballGo.AddComponent(ballType);
            var bounce = ballType.GetMethod("TryBounceAt");
            bounce.Invoke(ball, new object[] { 1f });
            Assert.That((int)managerType.GetProperty("CurrentScore").GetValue(manager), Is.Zero);
            bounce.Invoke(ball, new object[] { 1.01f });
            Assert.That((int)managerType.GetProperty("CurrentScore").GetValue(manager), Is.Zero);
            bounce.Invoke(ball, new object[] { 1.21f });
            Assert.That((int)managerType.GetProperty("CurrentScore").GetValue(manager), Is.Zero);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(ballGo);
            UnityEngine.Object.DestroyImmediate(managerGo);
            if (hadKey) PlayerPrefs.SetInt(key, previous); else PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
        }
    }

    [Test]
    public void Scores_PersistOnlyTheRecord_AndNewSessionStartsAtZero()
    {
        var type = Find("GameManager");
        Assert.That(type, Is.Not.Null, "Falta GameManager con récord persistente.");
        const string key = "HelixJump.HighScore";
        bool hadKey = PlayerPrefs.HasKey(key);
        int previous = PlayerPrefs.GetInt(key, 0);
        GameObject go = null;
        try
        {
            PlayerPrefs.SetInt(key, 12);
            go = new GameObject("ManagerTest");
            var manager = go.AddComponent(type);
            type.GetMethod("Awake", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(manager, null);
            Assert.That((int)type.GetProperty("CurrentScore").GetValue(manager), Is.EqualTo(0));
            Assert.That((int)type.GetProperty("BestScore").GetValue(manager), Is.EqualTo(12));
            type.GetMethod("AddScore").Invoke(manager, new object[] { 1 });
            Assert.That(PlayerPrefs.GetInt(key), Is.EqualTo(12), "Una puntuación menor no reduce el récord.");
            type.GetMethod("AddScore").Invoke(manager, new object[] { 20 });
            Assert.That((int)type.GetProperty("CurrentScore").GetValue(manager), Is.EqualTo(21));
            Assert.That(PlayerPrefs.GetInt(key), Is.EqualTo(21));
            UnityEngine.Object.DestroyImmediate(go);
            go = new GameObject("SecondSessionTest");
            manager = go.AddComponent(type);
            type.GetMethod("Awake", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(manager, null);
            Assert.That((int)type.GetProperty("CurrentScore").GetValue(manager), Is.EqualTo(0));
            Assert.That((int)type.GetProperty("BestScore").GetValue(manager), Is.EqualTo(21));
        }
        finally
        {
            if (go != null) UnityEngine.Object.DestroyImmediate(go);
            if (hadKey) PlayerPrefs.SetInt(key, previous); else PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
        }
    }

    [Test]
    public void Drag_RotatesOnlyY_AndNewPressDoesNotJump()
    {
        var type = Find("HelixController");
        Assert.That(type, Is.Not.Null, "Falta HelixController del capítulo 3.");
        var go = new GameObject("DragTest");
        try
        {
            var controller = go.AddComponent(type);
            var step = type.GetMethod("ProcessPointer");
            step.Invoke(controller, new object[] { new Vector2(100, 100), true });
            Assert.That(Quaternion.Angle(go.transform.localRotation, Quaternion.identity), Is.LessThan(0.001f));
            step.Invoke(controller, new object[] { new Vector2(160, 900), true });
            var rotation = go.transform.localRotation;
            Assert.That(Quaternion.Angle(rotation, Quaternion.identity), Is.GreaterThan(1));
            Assert.That(go.transform.up, Is.EqualTo(Vector3.up));
            step.Invoke(controller, new object[] { Vector2.zero, false });
            step.Invoke(controller, new object[] { new Vector2(900, 30), true });
            Assert.That(Quaternion.Angle(rotation, go.transform.localRotation), Is.LessThan(0.001f));
            step.Invoke(controller, new object[] { new Vector2(840, 30), true });
            Assert.That(Quaternion.Angle(go.transform.localRotation, Quaternion.identity), Is.LessThan(0.001f));
        }
        finally { UnityEngine.Object.DestroyImmediate(go); }
    }
}
