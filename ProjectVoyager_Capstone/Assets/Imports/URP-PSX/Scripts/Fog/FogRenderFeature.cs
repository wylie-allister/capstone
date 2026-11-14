using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

namespace PSX
{
    public class FogRenderFeature : ScriptableRendererFeature
    {
        FogPass fogPass;

        public override void Create()
        {
            fogPass = new FogPass(RenderPassEvent.BeforeRenderingPostProcessing);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(fogPass);
        }

        protected override void Dispose(bool disposing)
        {
            fogPass?.Dispose();
        }
    }


    public class FogPass : ScriptableRenderPass
    {
        private static readonly string shaderPath = "PostEffect/Fog";
        static readonly string k_RenderTag = "Render Fog Effects";
        static readonly int FogDensity = Shader.PropertyToID("_FogDensity");
        static readonly int FogDistance = Shader.PropertyToID("_FogDistance");
        static readonly int FogColor = Shader.PropertyToID("_FogColor");
        static readonly int FogNear = Shader.PropertyToID("_FogNear");
        static readonly int FogFar = Shader.PropertyToID("_FogFar");
        static readonly int FogAltScale = Shader.PropertyToID("_FogAltScale");
        static readonly int FogThinning = Shader.PropertyToID("_FogThinning");
        static readonly int NoiseScale = Shader.PropertyToID("_NoiseScale");
        static readonly int NoiseStrength = Shader.PropertyToID("_NoiseStrength");

        Fog fog;
        Material fogMaterial;

        public FogPass(RenderPassEvent evt)
        {
            renderPassEvent = evt;
            profilingSampler = new ProfilingSampler(k_RenderTag);

            var shader = Shader.Find(shaderPath);
            if (shader == null)
            {
                Debug.LogError("Shader not found.");
                return;
            }
            this.fogMaterial = CoreUtils.CreateEngineMaterial(shader);

            // Fog samples scene depth, so make sure the depth texture is generated
            ConfigureInput(ScriptableRenderPassInput.Depth);
        }

        // Replaces the old Execute(ScriptableRenderContext, ref RenderingData) entry point.
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            if (this.fogMaterial == null)
            {
                Debug.LogError("Material not created.");
                return;
            }

            var cameraData = frameData.Get<UniversalCameraData>();
            if (!cameraData.postProcessEnabled) return;

            var stack = VolumeManager.instance.stack;
            this.fog = stack.GetComponent<Fog>();
            if (this.fog == null) return;
            if (!this.fog.IsActive()) return;

            this.fogMaterial.SetFloat(FogDensity, this.fog.fogDensity.value);
            this.fogMaterial.SetFloat(FogDistance, this.fog.fogDistance.value);
            this.fogMaterial.SetColor(FogColor, this.fog.fogColor.value);
            this.fogMaterial.SetFloat(FogNear, this.fog.fogNear.value);
            this.fogMaterial.SetFloat(FogFar, this.fog.fogFar.value);
            this.fogMaterial.SetFloat(FogAltScale, this.fog.fogAltScale.value);
            this.fogMaterial.SetFloat(FogThinning, this.fog.fogThinning.value);
            this.fogMaterial.SetFloat(NoiseScale, this.fog.noiseScale.value);
            this.fogMaterial.SetFloat(NoiseStrength, this.fog.noiseStrength.value);

            var resourceData = frameData.Get<UniversalResourceData>();
            if (resourceData.isActiveTargetBackBuffer) return;

            var source = resourceData.activeColorTexture;

            var destinationDesc = renderGraph.GetTextureDesc(source);
            destinationDesc.name = "_TempTargetFog";
            destinationDesc.clearBuffer = false;
            TextureHandle destination = renderGraph.CreateTexture(destinationDesc);

            RenderGraphUtils.BlitMaterialParameters blitParams = new(source, destination, this.fogMaterial, 0);
            renderGraph.AddBlitPass(blitParams, k_RenderTag);

            resourceData.cameraColor = destination;
        }

        public void Dispose()
        {
            CoreUtils.Destroy(fogMaterial);
        }
    }
}