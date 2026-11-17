using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
public class HelixJump46Tests
{
 static Type Ball=>TypeCache.GetTypesDerivedFrom<MonoBehaviour>().Single(t=>t.FullName=="HelixJump.BallController");
 [Test] public void Combo_ThreePasses_ActivatesExactlyOnce_Resets()
 {
  var go=new GameObject("Combo46");go.transform.position=new Vector3(1000,1000,1000);
  try {
   var ball=go.AddComponent(Ball);var pass=Ball.GetMethod("RegisterPerfectPass");
   Assert.That(pass,Is.Not.Null,"Chapter11 missing: perfect-pass activation API");
   pass.Invoke(ball,null);pass.Invoke(ball,null);
   Assert.That(Ball.GetProperty("SuperSpeedActive").GetValue(ball),Is.False);
   pass.Invoke(ball,null);pass.Invoke(ball,null);
   Assert.That(Ball.GetProperty("SuperSpeedActive").GetValue(ball),Is.True);
   Assert.That(Ball.GetProperty("SuperActivationCount").GetValue(ball),Is.EqualTo(1));
   Ball.GetMethod("ResetBall").Invoke(ball,null);
   Assert.That(Ball.GetProperty("PerfectPassCount").GetValue(ball),Is.EqualTo(0));
   Assert.That(Ball.GetProperty("SuperSpeedActive").GetValue(ball),Is.False);
  } finally {UnityEngine.Object.DestroyImmediate(go);}
 }
}
