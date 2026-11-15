using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
namespace HelixJump.Editor
{
    public static class Chapter910Setup
    {
        const string Root="Assets/HelixJump/";
        const string GameScene=Root+"Scenes/HelixJump.unity";
        const string MenuScene=Root+"Scenes/MainMenu.unity";
        [MenuItem("Helix Jump/Incorporar capítulos 9 y 10")]
        public static void Apply()
        {
            if(Application.isPlaying)throw new InvalidOperationException("Detén Play antes de configurar.");
            var scene=EditorSceneManager.OpenScene(GameScene);
            var helix=GameObject.Find("Helix");var top=helix.transform.Find("HelixLevel_01");var goal=helix.transform.Find("HelixGoal");
            var safe=AssetDatabase.LoadAssetAtPath<Material>(Root+"Materials/Safe_Ivory_56.mat");
            foreach(var r in top.GetComponentsInChildren<MeshRenderer>(true))
            {
                var hazard=r.GetComponent<DeathPart>();if(hazard!=null)UnityEngine.Object.DestroyImmediate(hazard);
                r.SetPropertyBlock(null);r.sharedMaterial=safe;
            }
            var stages=new List<Stage>();
            for(int i=0;i<3;i++)
            {
                string path=Root+"Stages/Stage_Christmas_910_0"+(i+1)+".asset";
                var stage=AssetDatabase.LoadAssetAtPath<Stage>(path);
                if(stage==null)
                {
                    stage=ScriptableObject.CreateInstance<Stage>();
                    stage.backgroundColor=i==0?new Color32(184,225,211,255):i==1?new Color32(224,235,248,255):new Color32(244,222,207,255);
                    stage.ballColor=i==0?new Color32(22,163,74,255):i==1?new Color32(12,132,86,255):new Color32(27,121,62,255);
                    stage.helixColor=i==0?new Color32(245,242,226,255):i==1?new Color32(255,245,213,255):new Color32(221,245,232,255);
                    stage.levels=new List<Level>{new Level{partCount=8+Math.Min(i,1),deathCount=1+Math.Max(0,i-1)},new Level{partCount=9+Math.Min(i,1),deathCount=2},new Level{partCount=10+Math.Max(0,i-1),deathCount=2+Math.Min(i,1)}};
                    AssetDatabase.CreateAsset(stage,path);
                }
                stages.Add(stage);
            }
            var controller=helix.GetComponent<HelixController>();
            controller.ConfigureStage(top,goal,AssetDatabase.LoadAssetAtPath<GameObject>(Root+"Prefabs/HelixLevel_Chapter78.prefab"),stages,new List<GameObject>());
            EditorUtility.SetDirty(controller);
            var ball=GameObject.Find("Ball");
            var ballData=new SerializedObject(ball.GetComponent<BallController>());
            // 5.5 alcanza el techo en la separación existente; 4 mantiene margen.
            ballData.FindProperty("impulseForce").floatValue=4f;ballData.ApplyModifiedPropertiesWithoutUndo();
            var camera=Camera.main;var angles=camera.transform.eulerAngles;camera.transform.rotation=Quaternion.Euler(angles.x,-0.5f,8f);camera.allowMSAA=true;
            var lights=UnityEngine.Object.FindObjectsByType<Light>(FindObjectsSortMode.None);
            Light main=null;foreach(var light in lights)if(light.type==LightType.Directional){main=light;break;}
            if(main==null)main=new GameObject("Directional Light").AddComponent<Light>();
            main.type=LightType.Directional;main.intensity=0.5f;main.color=new Color(1f,0.96f,0.86f);main.shadows=LightShadows.None;main.transform.rotation=Quaternion.Euler(35,0,0);
            var fillObject=GameObject.Find("Christmas Fill Light");if(fillObject==null)fillObject=new GameObject("Christmas Fill Light");
            var fill=fillObject.GetComponent<Light>();if(fill==null)fill=fillObject.AddComponent<Light>();
            fill.type=LightType.Directional;fill.intensity=1f;fill.color=new Color(0.95f,0.98f,1f);fill.shadows=LightShadows.None;fill.transform.rotation=Quaternion.Euler(45,180,0);
            var trailObject=ball.transform.Find("BallTrail");
            if(trailObject==null){trailObject=new GameObject("BallTrail").transform;trailObject.SetParent(ball.transform,false);}
            var trail=trailObject.GetComponent<TrailRenderer>();if(trail==null)trail=trailObject.gameObject.AddComponent<TrailRenderer>();
            trail.time=0.25f;trail.minVertexDistance=0.2f;trail.widthMultiplier=0.18f;trail.widthCurve=AnimationCurve.Linear(0,1,1,0);
            trail.autodestruct=false;trail.shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;trail.receiveShadows=false;
            var gradient=new Gradient();gradient.SetKeys(new[]{new GradientColorKey(new Color(0.7f,1f,0.8f),0),new GradientColorKey(new Color(0.2f,0.8f,0.5f),1)},new[]{new GradientAlphaKey(0.9f,0),new GradientAlphaKey(0,1)});trail.colorGradient=gradient;
            string materialPath=Root+"Materials/BallTrail_910.mat";
            var mat=AssetDatabase.LoadAssetAtPath<Material>(materialPath);
            if(mat==null)
            {
                mat=new Material(Shader.Find("Universal Render Pipeline/Particles/Unlit"));
                mat.SetFloat("_Surface",1);mat.SetFloat("_ZWrite",0);mat.SetFloat("_SrcBlend",(int)UnityEngine.Rendering.BlendMode.SrcAlpha);mat.SetFloat("_DstBlend",(int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");mat.SetOverrideTag("RenderType","Transparent");mat.renderQueue=3000;AssetDatabase.CreateAsset(mat,materialPath);
            }
            trail.sharedMaterial=mat;
            EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);
            CreateMenu();
            var scenes=new List<EditorBuildSettingsScene>{new EditorBuildSettingsScene(MenuScene,true),new EditorBuildSettingsScene(GameScene,true)};
            foreach(var old in EditorBuildSettings.scenes)if(old.path!=MenuScene && old.path!=GameScene)scenes.Add(old);
            EditorBuildSettings.scenes=scenes.ToArray();AssetDatabase.SaveAssets();
            EditorSceneManager.OpenScene(MenuScene);
        }
        static void CreateMenu()
        {
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
            var camera=new GameObject("Main Camera").AddComponent<Camera>();camera.tag="MainCamera";camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color32(20,61,42,255);camera.gameObject.AddComponent<AudioListener>();
            var canvasObject=new GameObject("Canvas",typeof(RectTransform),typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));
            var canvas=canvasObject.GetComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;
            var scaler=canvasObject.GetComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1080,1920);scaler.matchWidthOrHeight=0.5f;
            var menu=canvasObject.AddComponent<MenuController>();
            var title=Label("GameTitle",canvasObject.transform,"Helix Jump",110,new Vector2(0,420),new Vector2(950,220));title.fontStyle=FontStyle.BoldAndItalic;title.rectTransform.localRotation=Quaternion.Euler(0,0,8);
            Button("PlayButton",canvasObject.transform,"Jugar",new Vector2(0,100),new Color32(34,136,78,255),menu.Play);
            Button("QuitButton",canvasObject.transform,"Salir",new Vector2(0,-120),new Color32(160,38,49,255),menu.Quit);
            var events=new GameObject("EventSystem",typeof(EventSystem),typeof(InputSystemUIInputModule));events.GetComponent<InputSystemUIInputModule>().AssignDefaultActions();
            EditorSceneManager.SaveScene(scene,MenuScene);
        }
        static Text Label(string name,Transform parent,string text,int size,Vector2 position,Vector2 dimensions)
        {
            var go=new GameObject(name,typeof(RectTransform),typeof(Text));go.transform.SetParent(parent,false);
            var label=go.GetComponent<Text>();label.text=text;label.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");label.fontSize=size;label.alignment=TextAnchor.MiddleCenter;label.color=new Color32(245,242,226,255);label.raycastTarget=false;
            var rect=go.GetComponent<RectTransform>();rect.anchorMin=rect.anchorMax=new Vector2(0.5f,0.5f);rect.sizeDelta=dimensions;rect.anchoredPosition=position;return label;
        }
        static void Button(string name,Transform parent,string text,Vector2 position,Color color,UnityAction action)
        {
            var go=new GameObject(name,typeof(RectTransform),typeof(Image),typeof(UnityEngine.UI.Button));go.transform.SetParent(parent,false);
            var rect=go.GetComponent<RectTransform>();rect.anchorMin=rect.anchorMax=new Vector2(0.5f,0.5f);rect.sizeDelta=new Vector2(660,150);rect.anchoredPosition=position;
            go.GetComponent<Image>().color=color;var button=go.GetComponent<UnityEngine.UI.Button>();var colors=button.colors;colors.normalColor=Color.white;colors.highlightedColor=new Color(1.15f,1.15f,1.15f);colors.pressedColor=new Color(0.75f,0.75f,0.75f);button.colors=colors;
            Label("Label",go.transform,text,64,Vector2.zero,new Vector2(620,140)).fontStyle=FontStyle.Bold;
            UnityEventTools.AddPersistentListener(button.onClick,action);
        }
    }
}
