using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
namespace HelixJump.Editor
{
 public static class GeometryRevisionSetup
 {
  public const string PrefabPath="Assets/HelixJump/Prefabs/HelixLevel_GeometryRevision.prefab";
  // Imported sector local Z is world Y (rotation -90 X). Scale a dedicated
  // parent in ring space, never root Y or mesh local Y (which changes radius).
  static void ThickenSectors(GameObject root)
  {
   foreach(var c in root.GetComponentsInChildren<MeshCollider>(true))
   {
    if(c.transform.parent.name.StartsWith("ThicknessY_"))continue;
    var pivot=new GameObject("ThicknessY_"+c.name).transform;
    pivot.SetParent(c.transform.parent,false);
    c.transform.SetParent(pivot,false);
    pivot.localScale=new Vector3(1,2,1);
   }
  }
  static float PhysicalHeight(MeshCollider c)
  {
   var m=c.transform.localToWorldMatrix;var size=c.sharedMesh.bounds.size;
   return Mathf.Abs(m.m10)*size.x+Mathf.Abs(m.m11)*size.y+Mathf.Abs(m.m12)*size.z;
  }
  static void MatchAnchorThickness(GameObject root,float target)
  {
   foreach(var c in root.GetComponentsInChildren<MeshCollider>(true))
   {
    // Scene anchors are connected prefab instances: reparenting their imported
    // children is forbidden. Local Z is vertical for these rotated sectors.
    if(Mathf.Abs(c.transform.TransformDirection(Vector3.forward).y)<0.9999f)
     throw new InvalidOperationException("Sector vertical axis is not local Z: "+c.name);
    var scale=c.transform.localScale;scale.z*=target/PhysicalHeight(c);c.transform.localScale=scale;
    if(PrefabUtility.IsPartOfPrefabInstance(c.transform))PrefabUtility.RecordPrefabInstancePropertyModifications(c.transform);
   }
  }
  static void MatchBothAnchors(HelixController controller)
  {
   var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
   float target=PhysicalHeight(prefab.GetComponentsInChildren<MeshCollider>(true)[0]);
   MatchAnchorThickness(controller.Top.gameObject,target);
   MatchAnchorThickness(controller.Goal.gameObject,target);
  }
  [MenuItem("Helix Jump/Igualar grosor de inicio y meta")]
  public static void FixAnchorThickness()
  {
   if(Application.isPlaying)throw new InvalidOperationException("Detener Play antes de configurar.");
   var scene=EditorSceneManager.OpenScene("Assets/HelixJump/Scenes/HelixJump.unity");
   MatchBothAnchors(GameObject.Find("Helix").GetComponent<HelixController>());
   Physics.SyncTransforms();EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);
  }
  [MenuItem("Helix Jump/Aplicar revisión geométrica previa al APK")]
  public static void Apply()
  {
   if(Application.isPlaying)throw new InvalidOperationException("Detener Play antes de configurar.");
   var source=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/HelixJump/Prefabs/HelixLevel_Chapter78.prefab");
   var copy=UnityEngine.Object.Instantiate(source);copy.name="HelixLevel_GeometryRevision";
   ThickenSectors(copy);
   // Root and planar scoring trigger deliberately retain unit scale.
   PrefabUtility.SaveAsPrefabAsset(copy,PrefabPath);UnityEngine.Object.DestroyImmediate(copy);
   var scene=EditorSceneManager.OpenScene("Assets/HelixJump/Scenes/HelixJump.unity");
   var controller=GameObject.Find("Helix").GetComponent<HelixController>();
   var stages=new List<Stage>();
   for(int i=1;i<=3;i++)
   {
    var stage=AssetDatabase.LoadAssetAtPath<Stage>("Assets/HelixJump/Stages/Stage_Christmas_910_0"+i+".asset");
    if(stage.levels.Count==3)
    {
     // Three copies per original entry preserve the gradual base difficulty.
     stage.levels=stage.levels.SelectMany(l=>Enumerable.Range(0,3).Select(_=>new Level{partCount=l.partCount,deathCount=l.deathCount})).ToList();
     EditorUtility.SetDirty(stage);
    }
    if(stage.levels.Count!=9)throw new InvalidOperationException("Expected exactly nine generated rings.");
    stages.Add(stage);
   }
   var top=controller.Top;var goal=controller.Goal;
   var position=goal.localPosition;position.y=top.localPosition.y-30f+0.1f;goal.localPosition=position;
   MatchBothAnchors(controller);
   controller.ConfigureStage(top,goal,AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath),stages,new List<GameObject>());
   var tower=controller.transform.Find("Tower");
   // Cylinder height two: retain original upper end at Y=5, lower end goal-3.
   float towerTop=5f;float towerBottom=goal.localPosition.y-3f;
   position=tower.localPosition;position.y=(towerTop+towerBottom)*0.5f;tower.localPosition=position;
   var scale=tower.localScale;scale.y=(towerTop-towerBottom)*0.5f;tower.localScale=scale;
   var ball=new SerializedObject(GameObject.Find("Ball").GetComponent<BallController>());
   ball.FindProperty("impulseForce").floatValue=4f;
   ball.FindProperty("perfectPassThreshold").intValue=3;
   ball.FindProperty("superSpeedImpulse").floatValue=8f;
   ball.ApplyModifiedPropertiesWithoutUndo();
   EditorUtility.SetDirty(controller);EditorSceneManager.MarkSceneDirty(scene);
   EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
  }
 }
}
