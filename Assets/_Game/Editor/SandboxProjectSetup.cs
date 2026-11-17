using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace SledDogs.Editor
{
    public static class SandboxProjectSetup
    {
        private const string SettingsFolder = "Assets/_Game/Settings";
        private const string RendererPath = SettingsFolder + "/SandboxRenderer.asset";
        private const string PipelinePath = SettingsFolder + "/SandboxURP.asset";
        private const string SandboxFolder = "Assets/_Game/Sandbox";
        private const string ScenePath = SandboxFolder + "/Sandbox.unity";
        private const string GroundMaterialPath = SandboxFolder + "/Ground.mat";

        public static void ConfigureRendering()
        {
            if (!AssetDatabase.IsValidFolder(SettingsFolder))
            {
                AssetDatabase.CreateFolder("Assets/_Game", "Settings");
            }

            var renderer = AssetDatabase.LoadAssetAtPath<UniversalRendererData>(RendererPath);
            if (renderer == null)
            {
                renderer = ScriptableObject.CreateInstance<UniversalRendererData>();
                AssetDatabase.CreateAsset(renderer, RendererPath);
            }

            var pipeline = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(PipelinePath);
            if (pipeline == null)
            {
                pipeline = UniversalRenderPipelineAsset.Create(renderer);
                AssetDatabase.CreateAsset(pipeline, PipelinePath);
            }

            GraphicsSettings.defaultRenderPipeline = pipeline;
            AssetDatabase.SaveAssets();
            Debug.Log("SledDogs URP renderer configured: " + PipelinePath);
        }

        public static void CreateSandboxScene()
        {
            if (!AssetDatabase.IsValidFolder(SandboxFolder))
            {
                AssetDatabase.CreateFolder("Assets/_Game", "Sandbox");
            }

            if (!AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath))
            {
                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
                ground.name = "Ground";
                ground.transform.localScale = new Vector3(5f, 1f, 5f);

                var material = AssetDatabase.LoadAssetAtPath<Material>(GroundMaterialPath);
                if (material == null)
                {
                    material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                    material.color = new Color(0.7f, 0.8f, 0.85f);
                    AssetDatabase.CreateAsset(material, GroundMaterialPath);
                }
                ground.GetComponent<Renderer>().sharedMaterial = material;

                var cameraObject = new GameObject("Main Camera");
                cameraObject.tag = "MainCamera";
                var camera = cameraObject.AddComponent<Camera>();
                cameraObject.AddComponent<AudioListener>();
                camera.transform.position = new Vector3(0f, 8f, -12f);
                camera.transform.LookAt(Vector3.zero);

                var lightObject = new GameObject("Directional Light");
                var light = lightObject.AddComponent<Light>();
                light.type = LightType.Directional;
                light.intensity = 1.5f;
                lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

                EditorSceneManager.SaveScene(scene, ScenePath);
            }

            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            AssetDatabase.SaveAssets();
            Debug.Log("SledDogs sandbox scene configured: " + ScenePath);
        }
    }
}
