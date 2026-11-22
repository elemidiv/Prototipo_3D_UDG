using System;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
public class HelixJumpGeometryRevisionTests
{
 static Type T(string n)=>TypeCache.GetTypesDerivedFrom<MonoBehaviour>().Single(t=>t.FullName=="HelixJump."+n);
 [Test] public void ThickPrefab_DoublesPhysicalHeight_WithoutChangingRadiusOrTrigger()
 {
  var original=UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/HelixJump/Prefabs/HelixLevel_Chapter78.prefab"));
  var revised=UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/HelixJump/Prefabs/HelixLevel_GeometryRevision.prefab"));
  try
  {
   original.transform.position=Vector3.one*2000;revised.transform.position=Vector3.one*2100;Physics.SyncTransforms();
   var a=original.GetComponentsInChildren<MeshCollider>();var b=revised.GetComponentsInChildren<MeshCollider>();
   Assert.That(a.Length,Is.EqualTo(12));Assert.That(b.Length,Is.EqualTo(12));
   for(int i=0;i<a.Length;i++)
   {
    Assert.That(b[i].bounds.size.y,Is.EqualTo(a[i].bounds.size.y*2).Within(0.001f));
    Assert.That(b[i].bounds.size.x,Is.EqualTo(a[i].bounds.size.x).Within(0.001f));
    Assert.That(b[i].bounds.size.z,Is.EqualTo(a[i].bounds.size.z).Within(0.001f));
   }
   var before=original.GetComponentInChildren<BoxCollider>();var after=revised.GetComponentInChildren<BoxCollider>();
   Assert.That(after.isTrigger,Is.True);Assert.That(after.size,Is.EqualTo(before.size));Assert.That(after.transform.localPosition,Is.EqualTo(before.transform.localPosition));
   float freeTravel=3f-b[0].bounds.size.y-0.3f;float jump=4f*4f/(2f*Mathf.Abs(Physics.gravity.y));
   Assert.That(freeTravel,Is.GreaterThan(jump+0.5f),"El rebote normal mantiene holgura bajo el anillo superior.");
  }
  finally {UnityEngine.Object.DestroyImmediate(original);UnityEngine.Object.DestroyImmediate(revised);}
 }
 [Test] public void ExpandedTower_CoversAllRings_AndKeepsStartAndGoalCoherent()
 {
  EditorSceneManager.OpenScene("Assets/HelixJump/Scenes/HelixJump.unity");var helix=GameObject.Find("Helix").transform;
  var top=helix.Find("HelixLevel_01");var goal=helix.Find("HelixGoal");var tower=helix.Find("Tower").GetComponent<Renderer>().bounds;
  Assert.That(top.localPosition.y,Is.EqualTo(3f).Within(0.001f));Assert.That(goal.localPosition.y,Is.EqualTo(-26.9f).Within(0.001f));
  Assert.That(tower.max.y,Is.GreaterThan(top.position.y));Assert.That(tower.min.y,Is.LessThan(goal.position.y));
  Assert.That(GameObject.Find("Ball").transform.position.y,Is.EqualTo(4f).Within(0.001f));
 }

 [Test] public void StartAndGoal_HaveSamePhysicalThicknessAsIntermediatePrefab()
 {
  EditorSceneManager.OpenScene("Assets/HelixJump/Scenes/HelixJump.unity");
  var reference=UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/HelixJump/Prefabs/HelixLevel_GeometryRevision.prefab"));reference.transform.position=Vector3.one*2000;
  try
  {
   Physics.SyncTransforms();float expected=reference.GetComponentsInChildren<MeshCollider>()[0].bounds.size.y;
   var helix=GameObject.Find("Helix").transform;
   foreach(var name in new[]{"HelixLevel_01","HelixGoal"})
    foreach(var collider in helix.Find(name).GetComponentsInChildren<MeshCollider>())
     Assert.That(collider.bounds.size.y,Is.EqualTo(expected).Within(0.001f),name+" debe tener el mismo grosor físico que el prefab intermedio.");
  }
  finally{UnityEngine.Object.DestroyImmediate(reference);}
 }
 [Test] public void ConnectedStages_GenerateNineRings_ThreeUnitsApart()
 {
  EditorSceneManager.OpenScene("Assets/HelixJump/Scenes/HelixJump.unity");
  var helix=GameObject.Find("Helix").GetComponent(T("HelixController"));
  var data=new SerializedObject(helix);var stages=data.FindProperty("stages");
  Assert.That(stages.arraySize,Is.EqualTo(3));
  for(int s=0;s<3;s++) {
   var stage=new SerializedObject(stages.GetArrayElementAtIndex(s).objectReferenceValue);
   Assert.That(stage.FindProperty("levels").arraySize,Is.EqualTo(9),"Cada Stage conectado debe triplicar sus tres anillos; Top no cuenta.");
   helix.GetType().GetMethod("LoadStage").Invoke(helix,new object[]{s});
   var rings=helix.transform.Cast<Transform>().Where(t=>t.name.StartsWith("GeneratedLevel_")).OrderByDescending(t=>t.localPosition.y).ToArray();
   Assert.That(rings.Length,Is.EqualTo(9));
   var top=(Transform)helix.GetType().GetProperty("Top").GetValue(helix);var goal=(Transform)helix.GetType().GetProperty("Goal").GetValue(helix);
   for(int i=0;i<9;i++)Assert.That(top.localPosition.y-rings[i].localPosition.y,Is.EqualTo(3f*(i+1)).Within(0.001f));
   Assert.That(rings[8].localPosition.y-goal.localPosition.y,Is.EqualTo(2.9f).Within(0.001f));
  }
  EditorSceneManager.OpenScene("Assets/HelixJump/Scenes/HelixJump.unity");
 }
}
