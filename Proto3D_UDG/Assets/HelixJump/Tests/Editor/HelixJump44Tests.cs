using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEditor;

public class HelixJump44Tests
{
    static Type Find(string n) => TypeCache.GetTypesDerivedFrom<MonoBehaviour>().First(t => t.FullName == "HelixJump." + n);
    static object Call(object o, string n, params object[] a) => o.GetType().GetMethod(n, BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance).Invoke(o,a);
    [Test]
    public void LoadStage_GeneratesBetweenAnchors_AndReloadOwnsOnlyGeneratedPlatforms()
    {
        var root=new GameObject("GenerationTest");
        var top=new GameObject("Top"); top.transform.SetParent(root.transform); top.transform.localPosition=Vector3.up*3;
        var goal=new GameObject("Goal"); goal.transform.SetParent(root.transform); goal.transform.localPosition=Vector3.down*3;
        var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/HelixJump/Prefabs/HelixLevel_Chapter56.prefab");
        var stageType=TypeCache.GetTypesDerivedFrom<ScriptableObject>().Single(t=>t.FullName=="HelixJump.Stage");
        var stage=ScriptableObject.CreateInstance(stageType);
        var original=UnityEngine.Random.state;
        try
        {
            var serialized=new SerializedObject(stage); var levels=serialized.FindProperty("levels"); levels.arraySize=3;
            for(int i=0;i<3;i++) { var l=levels.GetArrayElementAtIndex(i); l.FindPropertyRelative("partCount").intValue=8; l.FindPropertyRelative("deathCount").intValue=2; }
            serialized.ApplyModifiedPropertiesWithoutUndo();
            var controller=root.AddComponent(Find("HelixController"));
            var stages=Activator.CreateInstance(typeof(List<>).MakeGenericType(stageType)); ((System.Collections.IList)stages).Add(stage);
            Call(controller,"ConfigureStage",top.transform,goal.transform,prefab,stages,new List<GameObject>{top});
            Call(controller,"LoadStage",-999);
            var owned=new SerializedObject(controller).FindProperty("spawnedLevels");
            Assert.That(owned.arraySize,Is.EqualTo(3),"LoadStage must generate the stage list, not retain the fixed top.");
            for(int i=0;i<3;i++)
            {
                var platform=(GameObject)owned.GetArrayElementAtIndex(i).objectReferenceValue;
                Assert.That(platform.transform.parent,Is.EqualTo(root.transform));
                Assert.That(platform.transform.localPosition.y,Is.LessThan(3).And.GreaterThan(-3));
                var sectors=platform.GetComponentsInChildren<MeshRenderer>(true).Where(r=>r.GetComponent<MeshCollider>()!=null).ToArray();
                Assert.That(sectors.Length,Is.EqualTo(12));
                Assert.That(sectors.Count(r=>r.gameObject.activeSelf),Is.EqualTo(8));
                Assert.That(sectors.Count(r=>r.gameObject.activeSelf && r.GetComponent(Find("DeathPart"))!=null),Is.EqualTo(2));
                Assert.That(platform.GetComponentsInChildren<BoxCollider>().Count(c=>c.isTrigger),Is.EqualTo(1));
            }
            root.transform.Rotate(0,70,0); Call(controller,"LoadStage",999);
            Assert.That(Quaternion.Angle(root.transform.localRotation,Quaternion.identity),Is.LessThan(0.01));
            Assert.That(top!=null,Is.True);
            Assert.That(root.transform.childCount,Is.EqualTo(5),"Reload must not leave duplicate levels.");
        }
        finally { UnityEngine.Random.state=original; UnityEngine.Object.DestroyImmediate(root); UnityEngine.Object.DestroyImmediate(stage); }
    }
}
