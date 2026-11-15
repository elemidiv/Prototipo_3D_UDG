using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEditor;
public class HelixJump45Tests
{
    [Test] public void NextLevel_IncrementsOnce_PreservesScore_RestartKeepsLevel()
    {
        var type=TypeCache.GetTypesDerivedFrom<MonoBehaviour>().Single(t=>t.FullName=="HelixJump.GameManager");
        var go=new GameObject("Chapter910Test");
        bool had=PlayerPrefs.HasKey("HelixJump.HighScore"); int previous=PlayerPrefs.GetInt("HelixJump.HighScore");
        try {
            var manager=go.AddComponent(type);
            type.GetMethod("Awake",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(manager,null);
            type.GetMethod("AddScore").Invoke(manager,new object[]{7});
            type.GetMethod("NextLevel").Invoke(manager,null);
            Assert.That(type.GetProperty("CurrentLevel").GetValue(manager),Is.EqualTo(1));
            type.GetMethod("NextLevel").Invoke(manager,null);
            Assert.That(type.GetProperty("CurrentLevel").GetValue(manager),Is.EqualTo(1),"Duplicate goal contacts in one physics tick");
            Assert.That(type.GetProperty("CurrentScore").GetValue(manager),Is.EqualTo(7));
            type.GetMethod("RestartLevel").Invoke(manager,null);
            Assert.That(type.GetProperty("CurrentLevel").GetValue(manager),Is.EqualTo(1));
            Assert.That(type.GetProperty("CurrentScore").GetValue(manager),Is.EqualTo(0));
        } finally { UnityEngine.Object.DestroyImmediate(go); if(had) PlayerPrefs.SetInt("HelixJump.HighScore",previous); else PlayerPrefs.DeleteKey("HelixJump.HighScore"); PlayerPrefs.Save(); }
    }
}
