using System;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEditor;
public class HelixJump46RegressionTests
{
 static Type T(string n)=>TypeCache.GetTypesDerivedFrom<MonoBehaviour>().Single(t=>t.FullName=="HelixJump."+n);
 static object Call(Component c,string n,params object[] a)=>c.GetType().GetMethod(n).Invoke(c,a);
 static object Read(Component c,string n)=>c.GetType().GetProperty(n).GetValue(c);
 [Test] public void SuperImpulse_AddsToExistingFall_RatherThanSlowingIt()
 {
  var go=new GameObject("FallingBoostRegression");go.transform.position=Vector3.one*2000;var mode=Physics.simulationMode;
  try
  {
   var ball=go.AddComponent(T("BallController"));var rb=go.GetComponent<Rigidbody>();rb.useGravity=false;Physics.simulationMode=SimulationMode.Script;rb.linearVelocity=Vector3.down*12;
   for(int i=0;i<3;i++)Call(ball,"RegisterPerfectPass");Physics.Simulate(0.02f);
   Assert.That(rb.linearVelocity.y,Is.LessThan(-19.9f),"Un impulso descendente de 8 debe sumarse a la caída de 12, no sustituirla.");
  }
  finally{UnityEngine.Object.DestroyImmediate(go);Physics.simulationMode=mode;}
 }
 [Test] public void NormalHazard_IsNotIgnoredBySafeBounceCooldown()
 {
  var go=new GameObject("CooldownHazardRegression");go.transform.position=Vector3.one*2000;var danger=GameObject.CreatePrimitive(PrimitiveType.Cube);danger.transform.position=Vector3.one*2100;
  try
  {
   var ball=go.AddComponent(T("BallController"));danger.AddComponent(T("DeathPart"));
   Call(ball,"TryBounceAt",10f);Assert.That(Read(ball,"BounceCount"),Is.EqualTo(1));
   Call(ball,"HandlePlatformContact",danger.GetComponent<Collider>(),true,10.01f);
   Assert.That(Read(ball,"BounceCount"),Is.Zero,"El cooldown solo suprime rebotes duplicados, no concede invulnerabilidad ante una pieza roja.");
  }
  finally{UnityEngine.Object.DestroyImmediate(go);UnityEngine.Object.DestroyImmediate(danger);}
 }
}
