using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class HelixJump43PlayTests
{
    bool had;
    int previous;
    const string Key = "HelixJump.HighScore";
    static Type Find(string n) => UnityEditor.TypeCache.GetTypesDerivedFrom<MonoBehaviour>().First(t=>t.FullName=="HelixJump."+n);
    static object Call(object o,string n,params object[] a)=>o.GetType().GetMethod(n).Invoke(o,a);
    static object Get(object o,string n)=>o.GetType().GetProperty(n).GetValue(o);
    [UnityTest]
    public IEnumerator HelixJump_PhysicsPassageAndDeathRestart()
    {
        had=PlayerPrefs.HasKey(Key); previous=PlayerPrefs.GetInt(Key);
        // SessionState sobrevive a las recargas de dominio de Enter/ExitPlayMode.
        UnityEditor.SessionState.SetBool("HelixJump43Test.HadRecord",had);
        UnityEditor.SessionState.SetInt("HelixJump43Test.PreviousRecord",previous);
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/HelixJump/Scenes/HelixJump.unity");
        yield return new EnterPlayMode();
        var gm=UnityEngine.Object.FindFirstObjectByType(Find("GameManager"));
        Call(gm,"RestartLevel");
        // La bola de la escena no debe reiniciar la partida durante este ensayo aislado.
        foreach (var existing in UnityEngine.Object.FindObjectsByType<Rigidbody>(FindObjectsSortMode.None)) existing.isKinematic = true;
        var go=new GameObject("PhysicsTestBall"); go.transform.position=new Vector3(1000,10,1000);
        var ball=go.AddComponent(Find("BallController")); var rb=go.GetComponent<Rigidbody>();
        var gate=new GameObject("PhysicsTestPass"); gate.transform.position=new Vector3(1000,8,1000);
        var box=gate.AddComponent<BoxCollider>(); box.isTrigger=true; box.size=new Vector3(4,0.1f,4);
        var point=gate.AddComponent(Find("PassScorePoint"));
        rb.linearVelocity=Vector3.down*4;
        for(int i=0;i<60 && !(bool)Get(point,"Passed");i++) yield return new WaitForFixedUpdate();
        Assert.That(Get(point,"Passed"),Is.True,"El paso debe provenir de OnTriggerEnter real. pos="+rb.position+" v="+rb.linearVelocity+" gravity="+rb.useGravity+" kinematic="+rb.isKinematic+" sim="+Physics.simulationMode+" collider="+go.GetComponent<Collider>()+" gm="+gm+" scene="+go.scene.name);
        Assert.That(Get(gm,"CurrentScore"),Is.EqualTo(1));
        Assert.That(Call(point,"TryPass",go.GetComponent<Collider>()),Is.False);
        Call(gm,"RestartLevel");
        Assert.That(Get(point,"Passed"),Is.False);
        Assert.That(Get(gm,"CurrentScore"),Is.EqualTo(0));
        Assert.That(rb.position.y,Is.EqualTo(10).Within(0.01));
        Assert.That(rb.linearVelocity,Is.EqualTo(Vector3.zero));
        gate.SetActive(false);
        var danger=GameObject.CreatePrimitive(PrimitiveType.Cube); danger.name="PhysicsTestDeath";
        danger.transform.position=new Vector3(1000,8,1000); danger.transform.localScale=new Vector3(4,0.2f,4);
        danger.AddComponent(Find("DeathPart"));
        var properties=new MaterialPropertyBlock(); danger.GetComponent<Renderer>().GetPropertyBlock(properties);
        Assert.That(properties.GetColor("_BaseColor"),Is.EqualTo(Color.red));
        Call(gm,"AddScore",7); int best=(int)Get(gm,"BestScore");
        UnityEngine.TestTools.LogAssert.Expect(LogType.Log,"Pasamos de nivel"); Call(gm,"NextLevel");
        rb.linearVelocity=Vector3.down*4;
        for(int i=0;i<60 && (int)Get(gm,"CurrentScore")!=0;i++) yield return new WaitForFixedUpdate();
        Assert.That(Get(gm,"CurrentScore"),Is.EqualTo(0),"La colisión DeathPart debe reiniciar.");
        Assert.That(Get(gm,"BestScore"),Is.EqualTo(best));
        Assert.That(Get(gm,"GoalReached"),Is.False);
        Assert.That(rb.position.y,Is.GreaterThan(9.9));
        Assert.That(rb.linearVelocity.magnitude,Is.LessThan(0.3));
        Assert.That(Get(ball,"BounceCount"),Is.EqualTo(0),"DeathPart no debe dar impulso de rebote.");
        UnityEngine.Object.Destroy(go); UnityEngine.Object.Destroy(gate); UnityEngine.Object.Destroy(danger);
        yield return new ExitPlayMode();
    }
    [UnityTearDown]
    public IEnumerator Cleanup()
    {
        if(Application.isPlaying) yield return new ExitPlayMode();
        had=UnityEditor.SessionState.GetBool("HelixJump43Test.HadRecord",false);
        previous=UnityEditor.SessionState.GetInt("HelixJump43Test.PreviousRecord",0);
        if(had) PlayerPrefs.SetInt(Key,previous); else PlayerPrefs.DeleteKey(Key);
        PlayerPrefs.Save();
    }
}
