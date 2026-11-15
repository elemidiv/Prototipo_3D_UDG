using System;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
public class HelixJump43ConfigurationTests
{
    [Test]
    public void Stage_IsEditableData_AndSceneHasPassageAndActiveHazards()
    {
        var type=TypeCache.GetTypesDerivedFrom<ScriptableObject>().Single(t=>t.FullName=="HelixJump.Stage");
        Assert.That(type.GetCustomAttributes(typeof(CreateAssetMenuAttribute),false).Length,Is.EqualTo(1));
        var asset=AssetDatabase.LoadAssetAtPath("Assets/HelixJump/Stages/Stage_Example.asset",type);
        Assert.That(asset,Is.Not.Null);
        var data=new SerializedObject(asset);
        var sections=data.FindProperty("levels");
        Assert.That(sections.arraySize,Is.EqualTo(12));
        for(int i=0;i<sections.arraySize;i++)
        {
            Assert.That(sections.GetArrayElementAtIndex(i).FindPropertyRelative("partCount").intValue,Is.InRange(1,11));
            Assert.That(sections.GetArrayElementAtIndex(i).FindPropertyRelative("deathCount").intValue,Is.EqualTo(0));
        }
        Assert.That(data.FindProperty("backgroundColor"),Is.Not.Null);
        Assert.That(data.FindProperty("ballColor"),Is.Not.Null);
        Assert.That(data.FindProperty("helixColor"),Is.Not.Null);
        EditorSceneManager.OpenScene("Assets/HelixJump/Scenes/HelixJump.unity");
        var controller=GameObject.Find("Helix").GetComponent(TypeCache.GetTypesDerivedFrom<MonoBehaviour>().Single(t=>t.FullName=="HelixJump.HelixController"));
        var references=new SerializedObject(controller);
        foreach(var n in new[]{"top","goal","levelPrefab"}) Assert.That(references.FindProperty(n).objectReferenceValue,Is.Not.Null,n);
        Assert.That(references.FindProperty("stages").arraySize,Is.EqualTo(1));
        var all=UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
        var passages=all.Where(c=>c.GetType().FullName=="HelixJump.PassScorePoint").ToArray();
        Assert.That(passages.Length,Is.EqualTo(1),"Solo el ancla superior está preconstruida; las otras plataformas se generan en Play.");
        Assert.That(passages.All(c=>c.GetComponent<BoxCollider>().isTrigger),Is.True);
        Assert.That(all.Count(c=>c.GetType().FullName=="HelixJump.DeathPart"),Is.EqualTo(1));
        Assert.That(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/HelixJump/Prefabs/HelixLevel_Chapter56.prefab"),Is.Not.Null);
    }
}
