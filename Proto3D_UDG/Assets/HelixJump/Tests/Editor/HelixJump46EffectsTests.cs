using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
public class HelixJump46EffectsTests
{
 static Type T(string name)=>TypeCache.GetTypesDerivedFrom<MonoBehaviour>().Single(t=>t.FullName=="HelixJump."+name);
 static object Call(Component c,string name,params object[] args)=>c.GetType().GetMethod(name).Invoke(c,args);
 static object Read(Component c,string name)=>c.GetType().GetProperty(name).GetValue(c);
 static void Boost(Component b){Call(b,"RegisterPerfectPass");Call(b,"RegisterPerfectPass");Call(b,"RegisterPerfectPass");}
 [TestCase(3, -3, 3, 0)] [TestCase(3,-3,0,0.5f)] [TestCase(3,-3,-3,1)] [TestCase(3,3,0,0)] [TestCase(3,-3,-100,1)]
 public void Progress_NormalizesClampsAndGuards(float top,float goal,float y,float expected)
 {Assert.That(T("UIManager").GetMethod("NormalizeProgress").Invoke(null,new object[]{top,goal,y}),Is.EqualTo(expected));}
 [Test] public void UI_SliderNonInteractive_NoHandle_InitialZero_LevelsOneTwo()
 {
  EditorSceneManager.OpenScene("Assets/HelixJump/Scenes/HelixJump.unity");
  var ui=(Component)UnityEngine.Object.FindAnyObjectByType(T("UIManager"));var slider=(Slider)Read(ui,"ProgressSlider");
  Assert.That(slider,Is.Not.Null);Assert.That(slider.interactable,Is.False);Assert.That(slider.handleRect,Is.Null);Assert.That(slider.value,Is.Zero);
  Assert.That(GameObject.Find("CurrentLevel_1112").GetComponent<Text>().text,Is.EqualTo("1"));Assert.That(GameObject.Find("NextLevel_1112").GetComponent<Text>().text,Is.EqualTo("2"));
 }
 [Test] public void Bounce_DeduplicatesAndClearsCombo_EmitsDifferentBursts_ResetClears()
 {
  EditorSceneManager.OpenScene("Assets/HelixJump/Scenes/HelixJump.unity");var go=GameObject.Find("Ball");var ball=go.GetComponent(T("BallController"));var fx=go.GetComponent(T("ChristmasBallEffects"));
  var snow=(ParticleSystem)Read(fx,"BounceParticles");var confetti=(ParticleSystem)Read(fx,"SuperParticles");
  Boost(ball);Call(ball,"RegisterPerfectPass");Assert.That(Read(fx,"SuperBursts"),Is.EqualTo(1));Assert.That(confetti.particleCount,Is.GreaterThan(0));
  Assert.That(Call(ball,"TryBounceAt",10f),Is.True);Assert.That(Call(ball,"TryBounceAt",10.01f),Is.False);Assert.That(Read(fx,"BounceBursts"),Is.EqualTo(1));Assert.That(Read(ball,"PerfectPassCount"),Is.Zero);Assert.That(Read(ball,"SuperSpeedActive"),Is.False);
  Assert.That(snow.main.simulationSpace,Is.EqualTo(ParticleSystemSimulationSpace.World));Assert.That(confetti.main.simulationSpace,Is.EqualTo(ParticleSystemSimulationSpace.World));Assert.That(snow.main.startColor.gradient.colorKeys[0].color,Is.Not.EqualTo(confetti.main.startColor.gradient.colorKeys[0].color));
  Assert.That(go.GetComponentsInChildren<Rigidbody>(true).Length,Is.EqualTo(1));Assert.That(snow.GetComponent<ParticleSystemRenderer>().sharedMaterial.shader.isSupported,Is.True);
  Call(ball,"ResetBall");Assert.That(snow.particleCount+confetti.particleCount,Is.Zero);Assert.That(go.GetComponentInChildren<TrailRenderer>().widthMultiplier,Is.EqualTo(0.18f).Within(0.001f));
 }
 [UnityTest] public IEnumerator SuperSpeed_PhysicalSafeAndDangerousRing_OneDestructionAfterPointTwo_GoalProtected()
 {
  EditorSceneManager.OpenScene("Assets/HelixJump/Scenes/HelixJump.unity");
  yield return new EnterPlayMode();
  var original=GameObject.Find("Ball");original.SetActive(false);
  var ballType=T("BallController");
  for(int danger=0;danger<2;danger++)
  {
   var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/HelixJump/Prefabs/HelixLevel_GeometryRevision.prefab");
   var ring=UnityEngine.Object.Instantiate(prefab);ring.name="RealThickRing46";ring.transform.position=new Vector3(1000,1000,1000);ring.AddComponent(T("BreakablePlatform"));
   var sectors=ring.GetComponentsInChildren<MeshCollider>(true);foreach(var c in sectors){c.gameObject.SetActive(true);if(danger==1)c.gameObject.AddComponent(T("DeathPart"));}
   Physics.SyncTransforms();var sector=sectors[0];var spare=sectors[1].gameObject;
   var go=new GameObject("IsolatedSuper46");go.transform.position=new Vector3(sector.bounds.center.x,sector.bounds.max.y+1.2f,sector.bounds.center.z);var ball=go.AddComponent(ballType);go.GetComponent<SphereCollider>().radius=0.15f;
   Boost(ball);var rb=go.GetComponent<Rigidbody>();float start=Time.fixedTime;float deadline=Time.realtimeSinceStartup+30;
   while(Time.fixedTime<start+0.02f&&Time.realtimeSinceStartup<deadline)yield return null;
   Assert.That(rb.linearVelocity.y,Is.LessThan(-7),"8 impulse must move downward with CCD");
   while(ring!=null&&!(bool)Read(ring.GetComponent(T("BreakablePlatform")),"Breaking")&&Time.realtimeSinceStartup<deadline)yield return null;
   Assert.That(ring,Is.Not.Null,"Ring must persist briefly after impact");Assert.That(Read(ring.GetComponent(T("BreakablePlatform")),"Breaking"),Is.True);
   Assert.That(Read(ball,"SuperSpeedActive"),Is.False);Assert.That(Read(ball,"BounceCount"),Is.EqualTo(1));
   float bounceStep=Time.fixedTime;while(Time.fixedTime<=bounceStep&&Time.realtimeSinceStartup<deadline)yield return null;
   Assert.That(rb.linearVelocity.y,Is.GreaterThan(0),"Queued impulse is applied on the following physics step");
   float hit=Time.time;while(Time.time<hit+0.08f)yield return null;Assert.That(ring!=null,Is.True,"Do not destroy immediately");
   while(Time.time<hit+0.3f)yield return null;Assert.That(ring==null,Is.True,"Whole ring including spare sector must disappear");Assert.That(spare==null,Is.True);
   var goal=new GameObject("ProtectedGoal46");goal.transform.position=new Vector3(1100,1000,1000);goal.AddComponent(T("GoalController"));var col=goal.AddComponent<BoxCollider>();goal.AddComponent(T("BreakablePlatform"));
   Boost(ball);Call(ball,"HandlePlatformContact",col,false,Time.time+1);Assert.That(goal!=null,Is.True);Assert.That(Read(goal.GetComponent(T("BreakablePlatform")),"Breaking"),Is.False);
   UnityEngine.Object.Destroy(goal);UnityEngine.Object.Destroy(go);
  }
  yield return new ExitPlayMode();
 }
}
