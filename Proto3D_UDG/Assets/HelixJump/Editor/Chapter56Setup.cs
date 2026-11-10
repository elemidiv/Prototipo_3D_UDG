using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace HelixJump.Editor
{
    public static class Chapter56Setup
    {
        const string Root = "Assets/HelixJump/";
        [MenuItem("Helix Jump/Incorporar capítulos 5 y 6")]
        public static void Apply()
        {
            if (Application.isPlaying) throw new InvalidOperationException("Detén Play antes de configurar.");
            var scene = EditorSceneManager.OpenScene(Root + "Scenes/HelixJump.unity");
            var helix = GameObject.Find("Helix");
            var top = helix.transform.Find("HelixLevel_01");
            var lower = helix.transform.Find("HelixLevel_02");
            if (top == null || lower == null) throw new InvalidOperationException("Faltan plataformas originales.");
            var safe = Material("Safe_Ivory_56", new Color32(245,242,226,255));
            var death = Material("Death_Red_56", Color.red);
            // Variantes propias: nunca se modifica la malla o prefab importado.
            var source = AssetDatabase.LoadAssetAtPath<GameObject>(Root + "Prefabs/HelixLevel.prefab");
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(source);
            instance.name = "HelixLevel_Chapter56";
            Configure(instance.transform, safe, death);
            var prefab = PrefabUtility.SaveAsPrefabAsset(instance, Root + "Prefabs/HelixLevel_Chapter56.prefab");
            UnityEngine.Object.DestroyImmediate(instance);
            Configure(top, safe, death);
            Configure(lower, safe, death);
            // Mantener el material dorado de la meta de los capítulos anteriores.
            var gold = AssetDatabase.LoadAssetAtPath<Material>(Root + "Materials/Goal_Gold.mat");
            foreach (var goal in helix.GetComponentsInChildren<GoalController>(true))
                goal.GetComponent<Renderer>().sharedMaterial = gold;
            if (!AssetDatabase.IsValidFolder(Root + "Stages")) AssetDatabase.CreateFolder(Root.TrimEnd('/'), "Stages");
            string stagePath = Root + "Stages/Stage_Example.asset";
            var stage = AssetDatabase.LoadAssetAtPath<Stage>(stagePath);
            if (stage == null)
            {
                stage = ScriptableObject.CreateInstance<Stage>();
                for (int i=0; i<12; i++) stage.levels.Add(new Level { partCount=11, deathCount=0 });
                AssetDatabase.CreateAsset(stage, stagePath);
            }
            helix.GetComponent<HelixController>().ConfigureStage(top, helix.transform.Find("HelixGoal"), prefab, new List<Stage>{stage}, new List<GameObject>{top.gameObject,lower.gameObject});
            EditorUtility.SetDirty(helix.GetComponent<HelixController>());
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
        }
        static Material Material(string name, Color color)
        {
            string path=Root+"Materials/"+name+".mat";
            var mat=AssetDatabase.LoadAssetAtPath<Material>(path);
            if(mat==null) { mat=new Material(Shader.Find("Universal Render Pipeline/Lit")); AssetDatabase.CreateAsset(mat,path); }
            mat.SetColor("_BaseColor",color); EditorUtility.SetDirty(mat); return mat;
        }
        static void Configure(Transform platform, Material safe, Material death)
        {
            foreach (Transform part in platform)
            {
                var renderer=part.GetComponent<Renderer>();
                if(renderer==null) continue;
                bool lethal=part.name=="HelixPart_02";
                var oldDeath = part.GetComponent<DeathPart>();
                if (!lethal && oldDeath != null) UnityEngine.Object.DestroyImmediate(oldDeath);
                renderer.SetPropertyBlock(null);
                renderer.sharedMaterial=lethal?death:safe;
                if(lethal && part.GetComponent<DeathPart>()==null) part.gameObject.AddComponent<DeathPart>();
            }
            var point=platform.Find("PassScorePoint");
            if(point==null) { point=new GameObject("PassScorePoint").transform; point.SetParent(platform,false); }
            point.localPosition=new Vector3(0,-0.35f,0);
            var box=point.GetComponent<BoxCollider>(); if(box==null) box=point.gameObject.AddComponent<BoxCollider>();
            box.isTrigger=true; box.size=new Vector3(6,0.1f,6);
            if(point.GetComponent<PassScorePoint>()==null) point.gameObject.AddComponent<PassScorePoint>();
        }
    }
}
