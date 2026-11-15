using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
public class HelixJump44SafetyTests
{
    GameObject root; Component controller; ScriptableObject stage;
    static Type Find(string name)=>TypeCache.GetTypesDerivedFrom<MonoBehaviour>().Single(t=>t.FullName=="HelixJump."+name);
    object Call(string name,params object[] args)=>controller.GetType().GetMethod(name).Invoke(controller,args);
    [SetUp] public void Setup()
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
        root=new GameObject("StageSafety");
        var top=new GameObject("Top");top.transform.SetParent(root.transform);top.transform.localPosition=Vector3.up*3;
        var goal=new GameObject("Goal");goal.transform.SetParent(root.transform);goal.transform.localPosition=Vector3.down*3;
        stage=ScriptableObject.CreateInstance(TypeCache.GetTypesDerivedFrom<ScriptableObject>().Single(t=>t.FullName=="HelixJump.Stage"));
        var data=new SerializedObject(stage);data.FindProperty("levels").arraySize=1;data.ApplyModifiedPropertiesWithoutUndo();
        controller=root.AddComponent(Find("HelixController"));
        var list=Activator.CreateInstance(typeof(List<>).MakeGenericType(stage.GetType()));((System.Collections.IList)list).Add(stage);
        Call("ConfigureStage",top.transform,goal.transform,AssetDatabase.LoadAssetAtPath<GameObject>("Assets/HelixJump/Prefabs/HelixLevel_Chapter78.prefab"),list,new List<GameObject>());
    }
    [TearDown] public void Cleanup()
    { UnityEngine.Object.DestroyImmediate(root);UnityEngine.Object.DestroyImmediate(stage); }
    MeshRenderer[] Sectors()=>root.GetComponentsInChildren<MeshRenderer>(true).Where(r=>r.GetComponent<MeshCollider>()!=null).ToArray();
    [TestCase(-100,99,1,1)] [TestCase(100,-8,11,0)]
    public void InvalidCounts_AreBounded_WithoutDisablingPassage(int parts,int dangers,int expectedParts,int expectedDeaths)
    {
        var data=new SerializedObject(stage);var section=data.FindProperty("levels").GetArrayElementAtIndex(0);
        section.FindPropertyRelative("partCount").intValue=parts;section.FindPropertyRelative("deathCount").intValue=dangers;data.ApplyModifiedPropertiesWithoutUndo();
        Call("LoadStage",0);
        Assert.That(Sectors().Count(r=>r.gameObject.activeSelf),Is.EqualTo(expectedParts));
        Assert.That(Sectors().Count(r=>r.gameObject.activeSelf && r.GetComponent(Find("DeathPart"))!=null),Is.EqualTo(expectedDeaths));
        Assert.That(root.GetComponentsInChildren<BoxCollider>().Count(c=>c.isTrigger),Is.EqualTo(1));
    }
    [Test] public void StageColors_ReachCameraBallAndSafeSectors()
    {
        var camera=new GameObject("TestCamera").AddComponent<Camera>();camera.tag="MainCamera";camera.transform.SetParent(root.transform);
        var ball=GameObject.CreatePrimitive(PrimitiveType.Sphere);ball.transform.SetParent(root.transform);ball.AddComponent(Find("BallController"));
        var data=new SerializedObject(stage);data.FindProperty("backgroundColor").colorValue=Color.cyan;data.FindProperty("ballColor").colorValue=Color.green;data.FindProperty("helixColor").colorValue=Color.yellow;
        var section=data.FindProperty("levels").GetArrayElementAtIndex(0);section.FindPropertyRelative("partCount").intValue=8;section.FindPropertyRelative("deathCount").intValue=2;data.ApplyModifiedPropertiesWithoutUndo();
        Call("LoadStage",0);
        Assert.That(camera.backgroundColor,Is.EqualTo(Color.cyan));Assert.That(camera.clearFlags,Is.EqualTo(CameraClearFlags.SolidColor));
        var props=new MaterialPropertyBlock();ball.GetComponent<Renderer>().GetPropertyBlock(props);Assert.That(props.GetColor("_BaseColor"),Is.EqualTo(Color.green));
        foreach(var r in Sectors().Where(r=>r.gameObject.activeSelf))
        {
            // En EditMode Unity no ejecuta OnEnable de un MonoBehaviour normal.
            var danger=r.GetComponent(Find("DeathPart"));
            if(danger!=null) danger.GetType().GetMethod("OnEnable",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(danger,null);
            r.GetPropertyBlock(props);Assert.That(props.GetColor("_BaseColor"),Is.EqualTo(r.GetComponent(Find("DeathPart"))!=null?Color.red:Color.yellow)); }
    }
    [Test] public void EmptyAndNullStage_LeaveExistingStageIntact()
    {
        var data=new SerializedObject(stage);data.FindProperty("levels").arraySize=0;data.ApplyModifiedPropertiesWithoutUndo();
        Assert.DoesNotThrow(()=>Call("LoadStage",-1));Assert.That(Sectors().Length,Is.Zero);
        var references=new SerializedObject(controller);references.FindProperty("stages").arraySize=0;references.ApplyModifiedPropertiesWithoutUndo();
        Assert.DoesNotThrow(()=>Call("LoadStage",999));
        references.Update();references.FindProperty("stages").arraySize=1;references.FindProperty("stages").GetArrayElementAtIndex(0).objectReferenceValue=null;references.ApplyModifiedPropertiesWithoutUndo();
        Assert.DoesNotThrow(()=>Call("LoadStage",0));Assert.That(Sectors().Length,Is.Zero);
    }
}
