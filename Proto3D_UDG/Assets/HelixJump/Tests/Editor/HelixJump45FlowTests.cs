using System;
using System.Linq;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEditor;
using UnityEditor.SceneManagement;
public class HelixJump45FlowTests
{
    const string Key="HelixJump.HighScore";
    static Type Find(string name)=>TypeCache.GetTypesDerivedFrom<MonoBehaviour>().Single(t=>t.FullName=="HelixJump."+name);
    static object Call(object o,string method,params object[] args)=>o.GetType().GetMethod(method).Invoke(o,args);
    static int Value(object o,string property)=>(int)o.GetType().GetProperty(property).GetValue(o);
    [Test] public void Menu_HasPersistentButtons_InputAndBuildOrder()
    {
        EditorSceneManager.OpenScene("Assets/HelixJump/Scenes/MainMenu.unity");
        var buttons=UnityEngine.Object.FindObjectsByType<Button>(FindObjectsSortMode.None);
        Assert.That(buttons.Length,Is.EqualTo(2));
        Assert.That(buttons.Single(b=>b.name=="PlayButton").onClick.GetPersistentMethodName(0),Is.EqualTo("Play"));
        Assert.That(buttons.Single(b=>b.name=="QuitButton").onClick.GetPersistentMethodName(0),Is.EqualTo("Quit"));
        Assert.That(UnityEngine.Object.FindFirstObjectByType<CanvasScaler>().referenceResolution,Is.EqualTo(new Vector2(1080,1920)));
        Assert.That(UnityEngine.Object.FindFirstObjectByType<UnityEngine.EventSystems.BaseInputModule>().GetType().FullName,Is.EqualTo("UnityEngine.InputSystem.UI.InputSystemUIInputModule"));
        Assert.That(EditorBuildSettings.scenes[0].path,Is.EqualTo("Assets/HelixJump/Scenes/MainMenu.unity"));
        Assert.That(EditorBuildSettings.scenes[1].path,Is.EqualTo("Assets/HelixJump/Scenes/HelixJump.unity"));
        EditorSceneManager.OpenScene("Assets/HelixJump/Scenes/HelixJump.unity");
        var trail=GameObject.Find("Ball").GetComponentInChildren<TrailRenderer>();Assert.That(trail,Is.Not.Null);Assert.That(trail.time,Is.EqualTo(0.25f));Assert.That(trail.sharedMaterial.shader.isSupported,Is.True);
        var lights=UnityEngine.Object.FindObjectsByType<Light>(FindObjectsSortMode.None).Where(l=>l.type==LightType.Directional).ToArray();Assert.That(lights.Length,Is.EqualTo(2));Assert.That(lights.All(l=>l.shadows==LightShadows.None),Is.True);
    }
    [UnityTest] public IEnumerator MenuPlay_ToTwoConsecutivePhysicalGoals_AndRestart()
    {
        SessionState.SetBool("Helix45.Had",PlayerPrefs.HasKey(Key));SessionState.SetInt("Helix45.Record",PlayerPrefs.GetInt(Key));SessionState.SetBool("Helix45.Restore",true);
        EditorSceneManager.OpenScene("Assets/HelixJump/Scenes/MainMenu.unity");
        yield return new EnterPlayMode();yield return null;
        LogAssert.Expect(LogType.Log,"Salir del juego (Application.Quit solo cierra la compilación).");
        GameObject.Find("QuitButton").GetComponent<Button>().onClick.Invoke();
        GameObject.Find("PlayButton").GetComponent<Button>().onClick.Invoke();
        yield return null;yield return null;
        Assert.That(SceneManager.GetActiveScene().name,Is.EqualTo("HelixJump"));
        var manager=UnityEngine.Object.FindFirstObjectByType(Find("GameManager"));var ball=(Component)UnityEngine.Object.FindFirstObjectByType(Find("BallController"));
        var body=ball.GetComponent<Rigidbody>();body.collisionDetectionMode=CollisionDetectionMode.Discrete;
        Call(manager,"AddScore",7);Assert.That(Value(manager,"CurrentLevel"),Is.EqualTo(0));
        var trail=ball.GetComponentInChildren<TrailRenderer>();trail.AddPosition(Vector3.one*100);Call(ball,"ResetBall");Assert.That(trail.positionCount,Is.Zero);
        Color previous=Camera.main.backgroundColor;
        for(int expected=1;expected<=2;expected++)
        {
            var goal=GameObject.Find("Helix").transform.Find("HelixGoal");
            body.position=new Vector3(0,goal.position.y+0.6f,-1.5f);body.linearVelocity=Vector3.down*3;
            LogAssert.Expect(LogType.Log,"Pasamos de nivel");
            float deadline=Time.realtimeSinceStartup+30;
            while(Value(manager,"CurrentLevel")<expected && Time.realtimeSinceStartup<deadline)yield return new WaitForFixedUpdate();
            Assert.That(Value(manager,"CurrentLevel"),Is.EqualTo(expected),"Debe avanzar por colisión física con la meta.");
            Assert.That(Value(manager,"CurrentScore"),Is.EqualTo(7),"El avance conserva Score.");
            Assert.That(body.position.y,Is.GreaterThan(3.5f));
            Assert.That(Camera.main.backgroundColor,Is.Not.EqualTo(previous));previous=Camera.main.backgroundColor;
            var helix=GameObject.Find("Helix").transform;
            Assert.That(Enumerable.Range(0,helix.childCount).Count(i=>helix.GetChild(i).name.StartsWith("GeneratedLevel_")),Is.EqualTo(3));
            yield return new WaitForSeconds(0.15f);
        }
        int best=Value(manager,"BestScore");Call(manager,"RestartLevel");
        Assert.That(Value(manager,"CurrentLevel"),Is.EqualTo(2));Assert.That(Value(manager,"CurrentScore"),Is.Zero);Assert.That(Value(manager,"BestScore"),Is.EqualTo(best));Assert.That(Camera.main.backgroundColor,Is.EqualTo(previous));
        yield return new ExitPlayMode();
    }
    [UnityTearDown] public IEnumerator Cleanup()
    {
        if(Application.isPlaying)yield return new ExitPlayMode();
        // Solo la prueba de flujo cambia preferencias, y su copia sobrevive al reload.
        if(SessionState.GetBool("Helix45.Restore",false))
        {
            if(SessionState.GetBool("Helix45.Had",false))PlayerPrefs.SetInt(Key,SessionState.GetInt("Helix45.Record",0));else PlayerPrefs.DeleteKey(Key);
            PlayerPrefs.Save();SessionState.SetBool("Helix45.Restore",false);
        }
    }
}
