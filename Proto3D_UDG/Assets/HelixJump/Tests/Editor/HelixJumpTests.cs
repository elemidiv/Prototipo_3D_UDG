using System;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class HelixJumpTests
{
    static Type Find(string name) => UnityEditor.TypeCache.GetTypesDerivedFrom<MonoBehaviour>()
        .FirstOrDefault(t => t.FullName == name);

    [Test]
    public void ChristmasScene_HasTutorialPhysics_Prefabs_AndOnlyTitleUI()
    {
        const string path = "Assets/HelixJump/Scenes/HelixJump.unity";
        Assert.That(UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEditor.SceneAsset>(path), Is.Not.Null, "Falta la escena Helix Jump.");
        var scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(path);
        var roots = scene.GetRootGameObjects();
        var ball = roots.SelectMany(r => r.GetComponentsInChildren<Rigidbody>()).Single();
        Assert.That(ball.name, Is.EqualTo("Ball"));
        Assert.That(ball.GetComponent<SphereCollider>(), Is.Not.Null);
        Assert.That(ball.constraints.HasFlag(RigidbodyConstraints.FreezePositionX), Is.True);
        Assert.That(ball.constraints.HasFlag(RigidbodyConstraints.FreezePositionZ), Is.True);
        Assert.That(ball.constraints.HasFlag(RigidbodyConstraints.FreezeRotation), Is.True);
        var colliders = roots.SelectMany(r => r.GetComponentsInChildren<MeshCollider>(true)).ToArray();
        Assert.That(colliders.Length, Is.GreaterThanOrEqualTo(24));
        Assert.That(colliders.All(c => c.sharedMesh != null), Is.True, "Cada pieza requiere malla de colisión.");
        Assert.That(roots.SelectMany(r => r.GetComponentsInChildren<Canvas>()).Count(), Is.EqualTo(1));
        var labels = roots.SelectMany(r => r.GetComponentsInChildren<UnityEngine.UI.Text>()).ToArray();
        Assert.That(labels.Length, Is.EqualTo(1));
        Assert.That(labels[0].text, Is.EqualTo("Helix Jump"));
        Assert.That(UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/HelixJump/Prefabs/HelixLevel.prefab"), Is.Not.Null);
        Assert.That(UnityEditor.PlayerSettings.defaultInterfaceOrientation, Is.EqualTo(UnityEditor.UIOrientation.Portrait));
    }

    [Test]
    public void CameraFollow_PreservesInitialOffsetY_AndDoesNotMoveXZ()
    {
        var type = Find("HelixJump.CameraFollow");
        Assert.That(type, Is.Not.Null, "Falta el seguimiento vertical del capítulo 2.");
        var target = new GameObject("TestTarget");
        var camera = new GameObject("TestCamera");
        try
        {
            target.transform.position = new Vector3(0, 4, -1.5f);
            camera.transform.position = new Vector3(0, 7, -6.5f);
            var follow = camera.AddComponent(type);
            type.GetMethod("Configure").Invoke(follow, new object[] { target.transform });
            target.transform.position = new Vector3(4, -2, 9);
            type.GetMethod("FollowNow").Invoke(follow, null);
            Assert.That(camera.transform.position.x, Is.EqualTo(0).Within(0.001f));
            Assert.That(camera.transform.position.y, Is.EqualTo(1).Within(0.001f));
            Assert.That(camera.transform.position.z, Is.EqualTo(-6.5f).Within(0.001f));
        }
        finally { UnityEngine.Object.DestroyImmediate(camera); UnityEngine.Object.DestroyImmediate(target); }
    }

    [Test]
    public void BallController_AcceptsOneBounce_RejectsDuplicate_ThenRearms()
    {
        var type = Find("HelixJump.BallController");
        Assert.That(type, Is.Not.Null, "Falta el controlador de rebote del tutorial.");
        var ball = new GameObject("TestBall");
        var oldMode = Physics.simulationMode;
        Physics.simulationMode = SimulationMode.Script;
        try
        {
            ball.AddComponent<SphereCollider>();
            var rb = ball.AddComponent<Rigidbody>();
            rb.useGravity = false;
            var controller = ball.AddComponent(type);
            var bounce = type.GetMethod("TryBounceAt");
            Assert.That(bounce, Is.Not.Null);
            Assert.That((bool)bounce.Invoke(controller, new object[] { 1f }), Is.True);
            Physics.Simulate(0.02f);
            float firstVelocity = rb.linearVelocity.y;
            Assert.That(firstVelocity, Is.GreaterThan(0));
            Assert.That((bool)bounce.Invoke(controller, new object[] { 1.01f }), Is.False);
            Assert.That(rb.linearVelocity.y, Is.EqualTo(firstVelocity).Within(0.001f));
            Assert.That((bool)bounce.Invoke(controller, new object[] { 1.21f }), Is.True);
            Physics.Simulate(0.02f);
            Assert.That(rb.linearVelocity.y, Is.EqualTo(firstVelocity).Within(0.001f), "El rebote no debe acumular energía.");
        }
        finally { UnityEngine.Object.DestroyImmediate(ball); Physics.simulationMode = oldMode; }
    }
}
