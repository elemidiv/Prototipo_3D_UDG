using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace HelixJump.Editor
{
    public static class Chapter78Setup
    {
        const string Root="Assets/HelixJump/";
        [MenuItem("Helix Jump/Incorporar capítulos 7 y 8")]
        public static void Apply()
        {
            if(Application.isPlaying) throw new InvalidOperationException("Detén Play antes de configurar.");
            var scene=EditorSceneManager.OpenScene(Root+"Scenes/HelixJump.unity");
            var helix=GameObject.Find("Helix");
            var top=helix.transform.Find("HelixLevel_01");
            var goal=helix.transform.Find("HelixGoal");
            if(top==null || goal==null) throw new InvalidOperationException("Faltan las anclas.");
            var old=helix.transform.Find("HelixLevel_02");
            if(old!=null) UnityEngine.Object.DestroyImmediate(old.gameObject);
            var source=AssetDatabase.LoadAssetAtPath<GameObject>(Root+"Prefabs/HelixLevel_Chapter56.prefab");
            var instance=(GameObject)PrefabUtility.InstantiatePrefab(source);
            instance.name="HelixLevel_Chapter78";
            var safe=AssetDatabase.LoadAssetAtPath<Material>(Root+"Materials/Safe_Ivory_56.mat");
            foreach(var renderer in instance.GetComponentsInChildren<MeshRenderer>(true))
            {
                if(renderer.GetComponent<MeshCollider>()==null) continue;
                renderer.gameObject.SetActive(true);
                var danger=renderer.GetComponent<DeathPart>();
                if(danger!=null) UnityEngine.Object.DestroyImmediate(danger);
                renderer.SetPropertyBlock(null); renderer.sharedMaterial=safe;
            }
            var prefab=PrefabUtility.SaveAsPrefabAsset(instance,Root+"Prefabs/HelixLevel_Chapter78.prefab");
            UnityEngine.Object.DestroyImmediate(instance);
            string path=Root+"Stages/Stage_Christmas_78.asset";
            var stage=AssetDatabase.LoadAssetAtPath<Stage>(path);
            if(stage==null)
            {
                stage=ScriptableObject.CreateInstance<Stage>();
                stage.backgroundColor=new Color32(184,225,211,255);
                stage.ballColor=new Color32(22,163,74,255);
                stage.helixColor=new Color32(245,242,226,255);
                // Tres anillos caben entre las anclas de la torre existente.
                stage.levels=new List<Level>{new Level{partCount=8,deathCount=1},new Level{partCount=9,deathCount=2},new Level{partCount=10,deathCount=2}};
                AssetDatabase.CreateAsset(stage,path);
            }
            var controller=helix.GetComponent<HelixController>();
            controller.ConfigureStage(top,goal,prefab,new List<Stage>{stage},new List<GameObject>());
            EditorUtility.SetDirty(controller);
            EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
        }
    }
}
