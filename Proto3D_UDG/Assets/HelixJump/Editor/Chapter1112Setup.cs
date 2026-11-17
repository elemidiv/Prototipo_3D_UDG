using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
namespace HelixJump.Editor
{
 public static class Chapter1112Setup
 {
  const string Root="Assets/HelixJump/";
  [MenuItem("Helix Jump/Incorporar capítulos 11 y 12")]
  public static void Apply()
  {
   if(Application.isPlaying)throw new InvalidOperationException("Detén Play primero");
   var scene=EditorSceneManager.OpenScene(Root+"Scenes/HelixJump.unity");
   var ball=GameObject.Find("Ball");
   var data=new SerializedObject(ball.GetComponent<BallController>());data.FindProperty("perfectPassThreshold").intValue=3;data.FindProperty("superSpeedImpulse").floatValue=8;data.ApplyModifiedPropertiesWithoutUndo();
   var fx=ball.GetComponent<ChristmasBallEffects>();if(fx==null)fx=ball.AddComponent<ChristmasBallEffects>();
   string path=Root+"Materials/ChristmasParticles_1112.mat";
   var mat=AssetDatabase.LoadAssetAtPath<Material>(path);
   if(mat==null){mat=new Material(Shader.Find("Universal Render Pipeline/Particles/Unlit"));mat.SetFloat("_Surface",1);mat.SetFloat("_ZWrite",0);mat.SetFloat("_SrcBlend",5);mat.SetFloat("_DstBlend",10);mat.SetFloat("_Cull",0);mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");mat.SetOverrideTag("RenderType","Transparent");mat.renderQueue=3000;AssetDatabase.CreateAsset(mat,path);}
   fx.Configure(Particles(ball.transform,"BounceSnow_1112",false,SnowMaterial(mat)),Particles(ball.transform,"SuperConfetti_1112",true,mat));EditorUtility.SetDirty(fx);
   var ui=UnityEngine.Object.FindFirstObjectByType<UIManager>();var canvas=ui.GetComponentInParent<Canvas>();if(canvas==null)canvas=UnityEngine.Object.FindFirstObjectByType<Canvas>();
   var scaler=canvas.GetComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1080,1920);scaler.matchWidthOrHeight=0.5f;
   foreach(var label in canvas.GetComponentsInChildren<Text>(true))
   {
    string n=label.name.ToLowerInvariant();
    if(n.Contains("title")){Place(label.rectTransform,new Vector2(0,-100),new Vector2(1000,100));label.fontSize=74;}
    else if(n.Contains("best")){Place(label.rectTransform,new Vector2(230,-330),new Vector2(440,90));label.fontSize=45;}
    else if(n.Contains("score")){Place(label.rectTransform,new Vector2(-230,-330),new Vector2(440,90));label.fontSize=45;}
   }
   var old=canvas.transform.Find("Progress_1112");if(old!=null)UnityEngine.Object.DestroyImmediate(old.gameObject);
   var bar=new GameObject("Progress_1112",typeof(RectTransform),typeof(Image),typeof(Slider));bar.transform.SetParent(canvas.transform,false);
   Place(bar.GetComponent<RectTransform>(),new Vector2(0,-230),new Vector2(650,44));bar.GetComponent<Image>().color=new Color32(27,74,49,255);bar.GetComponent<Image>().raycastTarget=false;
   var fill=new GameObject("Fill",typeof(RectTransform),typeof(Image));fill.transform.SetParent(bar.transform,false);var rect=fill.GetComponent<RectTransform>();rect.anchorMin=Vector2.zero;rect.anchorMax=Vector2.one;rect.offsetMin=new Vector2(5,5);rect.offsetMax=new Vector2(-5,-5);fill.GetComponent<Image>().color=new Color32(229,186,73,255);fill.GetComponent<Image>().raycastTarget=false;
   var slider=bar.GetComponent<Slider>();slider.interactable=false;slider.transition=Selectable.Transition.None;slider.minValue=0;slider.maxValue=1;slider.wholeNumbers=false;slider.fillRect=rect;slider.handleRect=null;slider.direction=Slider.Direction.LeftToRight;slider.value=0;
   foreach(string name in new[]{"CurrentLevel_1112","NextLevel_1112"}){var prev=canvas.transform.Find(name);if(prev!=null)UnityEngine.Object.DestroyImmediate(prev.gameObject);}
   var current=Label(canvas.transform,"CurrentLevel_1112",new Vector2(-420,-230));var next=Label(canvas.transform,"NextLevel_1112",new Vector2(420,-230));current.text="1";next.text="2";
   ui.ConfigureProgress(slider,current,next);EditorUtility.SetDirty(ui);
   EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
  }
  static Material SnowMaterial(Material source)
  {
   string folder=Root+"Textures";if(!AssetDatabase.IsValidFolder(folder))AssetDatabase.CreateFolder(Root.TrimEnd('/'),"Textures");
   string texturePath=folder+"/Snowflake_1112.png";
   if(AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath)==null)
   {
    const int size=128;var texture=new Texture2D(size,size,TextureFormat.RGBA32,false);var pixels=new Color[size*size];
    for(int y=0;y<size;y++)for(int x=0;x<size;x++)
    {
     var point=new Vector2((x+0.5f)/size*2-1,(y+0.5f)/size*2-1);float distance=10;
     for(int arm=0;arm<6;arm++)
     {
      float angle=arm*Mathf.PI/3;var direction=new Vector2(Mathf.Cos(angle),Mathf.Sin(angle));var side=new Vector2(-direction.y,direction.x);
      distance=Mathf.Min(distance,SegmentDistance(point,Vector2.zero,direction*0.85f));
      distance=Mathf.Min(distance,SegmentDistance(point,direction*0.43f,direction*0.64f+side*0.2f));
      distance=Mathf.Min(distance,SegmentDistance(point,direction*0.43f,direction*0.64f-side*0.2f));
     }
     var color=Color.Lerp(Color.white,new Color(1,0.78f,0.3f),Mathf.Clamp01(point.magnitude));color.a=Mathf.Clamp01((0.043f-distance)*size*0.8f);pixels[y*size+x]=color;
    }
    texture.SetPixels(pixels);texture.Apply();System.IO.File.WriteAllBytes(texturePath,texture.EncodeToPNG());UnityEngine.Object.DestroyImmediate(texture);AssetDatabase.ImportAsset(texturePath);
    var importer=(TextureImporter)AssetImporter.GetAtPath(texturePath);importer.alphaIsTransparency=true;importer.mipmapEnabled=false;importer.wrapMode=TextureWrapMode.Clamp;importer.SaveAndReimport();
   }
   string path=Root+"Materials/ChristmasSnow_1112.mat";var mat=AssetDatabase.LoadAssetAtPath<Material>(path);
   if(mat==null){mat=new Material(source);AssetDatabase.CreateAsset(mat,path);}
   mat.SetTexture("_BaseMap",AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath));EditorUtility.SetDirty(mat);return mat;
  }
  static float SegmentDistance(Vector2 p,Vector2 a,Vector2 b)
  {var delta=b-a;return (p-(a+delta*Mathf.Clamp01(Vector2.Dot(p-a,delta)/delta.sqrMagnitude))).magnitude;}
  static void Place(RectTransform rect,Vector2 position,Vector2 size){rect.anchorMin=rect.anchorMax=new Vector2(0.5f,1);rect.pivot=new Vector2(0.5f,0.5f);rect.anchoredPosition=position;rect.sizeDelta=size;}
  static Text Label(Transform parent,string name,Vector2 position){var go=new GameObject(name,typeof(RectTransform),typeof(Text));go.transform.SetParent(parent,false);var text=go.GetComponent<Text>();text.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");text.fontSize=50;text.fontStyle=FontStyle.Bold;text.alignment=TextAnchor.MiddleCenter;text.color=new Color32(31,84,51,255);text.raycastTarget=false;Place(text.rectTransform,position,new Vector2(100,90));return text;}
  static ParticleSystem Particles(Transform parent,string name,bool super,Material material)
  {
   var old=parent.Find(name);if(old!=null)UnityEngine.Object.DestroyImmediate(old.gameObject);
   var go=new GameObject(name);go.transform.SetParent(parent,false);var ps=go.AddComponent<ParticleSystem>();ps.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
   var main=ps.main;main.playOnAwake=false;main.loop=false;main.duration=1;main.startLifetime=super?0.8f:0.65f;main.startSpeed=super?2.2f:1.1f;main.startSize=super?0.1f:0.13f;main.gravityModifier=super?0.35f:0.12f;main.simulationSpace=ParticleSystemSimulationSpace.World;main.maxParticles=128;
   var colors=new Gradient();colors.SetKeys(super?new[]{new GradientColorKey(new Color(0.85f,0.03f,0.08f),0),new GradientColorKey(new Color(0.05f,0.65f,0.16f),0.5f),new GradientColorKey(new Color(1,0.75f,0.13f),1)}:new[]{new GradientColorKey(Color.white,0),new GradientColorKey(new Color(1,0.94f,0.74f),1)},new[]{new GradientAlphaKey(1,0),new GradientAlphaKey(1,1)});main.startColor=new ParticleSystem.MinMaxGradient(colors){mode=ParticleSystemGradientMode.RandomColor};
   var emission=ps.emission;emission.enabled=false;var shape=ps.shape;shape.enabled=true;shape.shapeType=super?ParticleSystemShapeType.Sphere:ParticleSystemShapeType.Cone;shape.radius=super?0.11f:0.32f;if(!super){shape.radiusThickness=0.1f;shape.angle=65;go.transform.localRotation=Quaternion.Euler(-90,0,0);}
   var fade=ps.colorOverLifetime;fade.enabled=true;var g=new Gradient();g.SetKeys(new[]{new GradientColorKey(Color.white,0),new GradientColorKey(Color.white,1)},new[]{new GradientAlphaKey(1,0),new GradientAlphaKey(0,1)});fade.color=g;
   var renderer=go.GetComponent<ParticleSystemRenderer>();renderer.sharedMaterial=material;renderer.renderMode=super?ParticleSystemRenderMode.Stretch:ParticleSystemRenderMode.Billboard;renderer.lengthScale=super?1.5f:1;renderer.velocityScale=0.05f;renderer.shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;return ps;
  }
 }
}
