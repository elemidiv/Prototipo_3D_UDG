using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace HelixJump.Editor
{
    /// <summary>Construye exclusivamente la práctica de los capítulos 1 y 2.</summary>
    public static class TutorialSceneBuilder
    {
        const string Root = "Assets/HelixJump";
        const string ScenePath = Root + "/Scenes/HelixJump.unity";

        [MenuItem("Helix Jump/Crear escena de los tutoriales")]
        public static void Build()
        {
            if (File.Exists(ScenePath)) throw new InvalidOperationException("La escena ya existe. No se sobrescribe automáticamente.");
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty)
                throw new InvalidOperationException("Guarda la escena actual antes de crear la práctica.");
            foreach (var dir in new[] { "Scenes", "Materials", "Prefabs", "Physics" })
                Directory.CreateDirectory(Root + "/" + dir);
            AssetDatabase.Refresh();
            var source = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/HelixPartPrefab.prefab");
            if (source == null) throw new InvalidOperationException("No está el prefab importado del tutorial.");
            var mesh = source.GetComponent<MeshFilter>().sharedMesh;
            if (mesh == null) throw new InvalidOperationException("El recurso del tutorial no tiene malla.");

            var green = Material("Ball_Emerald", new Color32(27, 185, 83, 255), 0.28f, 0.08f);
            var red = Material("Platform_Cranberry", new Color32(183, 31, 53, 255), 0.17f, 0);
            var snow = Material("Platform_Snow", new Color32(245, 242, 226, 255), 0.12f, 0);
            var pine = Material("Tower_Pine", new Color32(23, 70, 54, 255), 0.18f, 0);
            var physics = new PhysicsMaterial("ControlledBounce")
            {
                bounciness = 0,
                dynamicFriction = 0,
                staticFriction = 0,
                bounceCombine = PhysicsMaterialCombine.Minimum,
                frictionCombine = PhysicsMaterialCombine.Minimum
            };
            AssetDatabase.CreateAsset(physics, Root + "/Physics/ControlledBounce.physicMaterial");
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.78f, 0.83f, 0.78f);
            RenderSettings.fog = false;

            var helix = new GameObject("Helix");
            var tower = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            tower.name = "Tower";
            tower.transform.SetParent(helix.transform, false);
            tower.transform.position = new Vector3(0, -15, 0);
            tower.transform.localScale = new Vector3(2, 20, 2);
            tower.GetComponent<Renderer>().sharedMaterial = pine;

            // Se reutiliza la geometría FBX del tutorial; no se altera el recurso original.
            var level = new GameObject("HelixLevel");
            for (int i = 0; i < 12; i++)
            {
                var part = (GameObject)PrefabUtility.InstantiatePrefab(source);
                part.name = "HelixPart_" + i.ToString("00");
                part.transform.SetParent(level.transform, false);
                part.transform.localPosition = Vector3.zero;
                part.transform.localRotation = Quaternion.Euler(-90, i * 30, 0);
                part.transform.localScale = new Vector3(3, 3, 0.75f);
                var collider = part.GetComponent<MeshCollider>();
                collider.sharedMesh = mesh; // El paquete original trae una referencia de collider no resuelta.
                collider.convex = false;
                collider.sharedMaterial = physics;
                part.GetComponent<Renderer>().sharedMaterial = i % 2 == 0 ? red : snow;
                part.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.Off;
            }
            var levelPrefab = PrefabUtility.SaveAsPrefabAsset(level, Root + "/Prefabs/HelixLevel.prefab");
            UnityEngine.Object.DestroyImmediate(level);
            for (int i = 0; i < 2; i++)
            {
                var instance = (GameObject)PrefabUtility.InstantiatePrefab(levelPrefab);
                instance.name = "HelixLevel_" + (i + 1).ToString("00");
                instance.transform.SetParent(helix.transform, false);
                instance.transform.localPosition = new Vector3(0, 3 - 3 * i, 0);
                // Hueco visible alejado del punto de rebote. Se pueden desactivar otras piezas para probar la caída.
                if (i == 0)
                {
                    instance.transform.Find("HelixPart_04").gameObject.SetActive(false);
                    instance.transform.Find("HelixPart_05").gameObject.SetActive(false);
                }
            }

            var ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            ball.name = "Ball";
            ball.transform.position = new Vector3(0, 4, -1.5f);
            ball.transform.localScale = Vector3.one * 0.3f;
            ball.GetComponent<Renderer>().sharedMaterial = green;
            ball.GetComponent<SphereCollider>().sharedMaterial = physics;
            var body = ball.AddComponent<Rigidbody>();
            body.mass = 1;
            body.useGravity = true;
            body.linearDamping = 0;
            body.angularDamping = 0.05f;
            body.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
            body.interpolation = RigidbodyInterpolation.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            ball.AddComponent<BallController>();
            PrefabUtility.SaveAsPrefabAsset(ball, Root + "/Prefabs/Ball.prefab");

            var cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0, 7, -6.5f);
            cameraObject.transform.rotation = Quaternion.Euler(30, 0, 0);
            var camera = cameraObject.GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color32(223, 237, 230, 255);
            camera.fieldOfView = 60;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 100;
            cameraObject.AddComponent<CameraFollow>().Configure(ball.transform);

            var light = new GameObject("Directional Light", typeof(Light)).GetComponent<Light>();
            light.type = LightType.Directional;
            light.color = new Color32(255, 244, 221, 255);
            light.intensity = 1.2f;
            light.shadows = LightShadows.Soft;
            light.transform.rotation = Quaternion.Euler(45, -30, 0);

            var canvasObject = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler));
            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            var titleObject = new GameObject("GameTitle", typeof(RectTransform), typeof(Text), typeof(Outline));
            titleObject.transform.SetParent(canvasObject.transform, false);
            var title = titleObject.GetComponent<Text>();
            title.text = "Helix Jump";
            title.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            title.fontSize = 94;
            title.fontStyle = FontStyle.Bold;
            title.alignment = TextAnchor.MiddleCenter;
            title.color = new Color32(22, 74, 47, 255);
            title.raycastTarget = false;
            var rt = title.rectTransform;
            rt.anchorMin = new Vector2(0.06f, 0.86f);
            rt.anchorMax = new Vector2(0.94f, 0.97f);
            rt.offsetMin = rt.offsetMax = Vector2.zero;
            var outline = titleObject.GetComponent<Outline>();
            outline.effectColor = new Color32(232, 199, 118, 255);
            outline.effectDistance = new Vector2(2, -2);

            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.productName = "Helix Jump";
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath));
            Debug.Log("Helix Jump: escena navideña creada con recursos originales y física de los capítulos 1 y 2.");
        }

        static Material Material(string name, Color color, float smoothness, float metallic)
        {
            var material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            material.name = name;
            material.SetColor("_BaseColor", color);
            material.SetFloat("_Smoothness", smoothness);
            material.SetFloat("_Metallic", metallic);
            AssetDatabase.CreateAsset(material, Root + "/Materials/" + name + ".mat");
            return material;
        }
    }
}
