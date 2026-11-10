using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEditor;

public class HelixJump43Tests
{
    static Type Find(string n) => TypeCache.GetTypesDerivedFrom<MonoBehaviour>().FirstOrDefault(t => t.FullName == "HelixJump." + n);
    static object Call(object o, string n, params object[] args) => o.GetType().GetMethod(n, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Invoke(o,args);
    static object Get(object o, string n) => o.GetType().GetProperty(n).GetValue(o);

    [Test]
    public void Passage_OnlyBallScoresOnce_AndRestartRearmsForReplay()
    {
        var type = Find("PassScorePoint");
        Assert.That(type, Is.Not.Null, "Falta el trigger de puntuación del capítulo 5");
        var gmGo = new GameObject("PassageManager");
        var ballGo = new GameObject("PassageBall");
        var passGo = new GameObject("PassageTest");
        var other = new GameObject("Other",typeof(BoxCollider));
        const string key="HelixJump.HighScore";
        bool had=PlayerPrefs.HasKey(key); int previous=PlayerPrefs.GetInt(key);
        try
        {
            var gm=gmGo.AddComponent(Find("GameManager")); Call(gm,"Awake");
            ballGo.transform.position=Vector3.up*10;
            var ball=ballGo.AddComponent(Find("BallController")); Call(ball,"Awake");
            var pass=passGo.AddComponent(type);
            var body=ballGo.GetComponent<Rigidbody>(); body.linearVelocity=Vector3.down;
            ballGo.transform.position=Vector3.down;
            Assert.That(Call(pass,"TryPass",other.GetComponent<Collider>()),Is.False);
            Assert.That(Call(pass,"TryPass",ballGo.GetComponent<Collider>()),Is.True);
            Assert.That(Call(pass,"TryPass",ballGo.GetComponent<Collider>()),Is.False);
            Assert.That(Get(gm,"CurrentScore"),Is.EqualTo(1));
            Call(gm,"RestartLevel");
            Assert.That(Get(gm,"CurrentScore"),Is.EqualTo(0));
            ballGo.transform.position=Vector3.down; body.linearVelocity=Vector3.down;
            Assert.That(Call(pass,"TryPass",ballGo.GetComponent<Collider>()),Is.True);
            Assert.That(Get(gm,"CurrentScore"),Is.EqualTo(1));
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(passGo); UnityEngine.Object.DestroyImmediate(ballGo);
            UnityEngine.Object.DestroyImmediate(gmGo); UnityEngine.Object.DestroyImmediate(other);
            if(had)PlayerPrefs.SetInt(key,previous);else PlayerPrefs.DeleteKey(key); PlayerPrefs.Save();
        }
    }
}
