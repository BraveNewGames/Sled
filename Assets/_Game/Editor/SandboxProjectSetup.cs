using UnityEditor;
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
    }
}
