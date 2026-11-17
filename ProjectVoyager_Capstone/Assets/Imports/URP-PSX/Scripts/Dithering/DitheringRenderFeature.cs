using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

namespace PSX
{
    public class DitheringRenderFeature : ScriptableRendererFeature
    {
        DitheringPass ditheringPass;

        public override void Create()
        {
            ditheringPass = new DitheringPass(RenderPassEvent.BeforeRenderingPostProcessing);
        }

        // ScriptableRendererFeature is an abstract class, you need this method.
        // NOTE: this legacy signature (ref RenderingData) still exists on ScriptableRendererFeature
        // itself in URP 17 for setup purposes, but the pass no longer overrides Execute — see below.
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(ditheringPass);
        }

        protected override void Dispose(bool disposing)
        {
            ditheringPass?.Dispose();
        }
    }


    public class DitheringPass : ScriptableRenderPass
    {
        private static readonly string shaderPath = "PostEffect/Dithering";
        static readonly string k_RenderTag = "Render Dithering Effects";

        //PROPERTIES
        static readonly int PatternIndex = Shader.PropertyToID("_PatternIndex");
        static readonly int DitherThreshold = Shader.PropertyToID("_DitherThreshold");
        static readonly int DitherStrength = Shader.PropertyToID("_DitherStrength");
        static readonly int DitherScale = Shader.PropertyToID("_DitherScale");

        Dithering dithering;
        Material ditheringMaterial;

        public DitheringPass(RenderPassEvent evt)
        {
            renderPassEvent = evt;
            profilingSampler = new ProfilingSampler(k_RenderTag);

            var shader = Shader.Find(shaderPath);
            if (shader == null)
            {
                Debug.LogError("Shader not found.");
                return;
            }
            this.ditheringMaterial = CoreUtils.CreateEngineMaterial(shader);
        }

        // Replaces the old Execute(ScriptableRenderContext, ref RenderingData) entry point.
        // This is the Render Graph equivalent used by URP 17+ (Unity 6).
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            if (this.ditheringMaterial == null)
            {
                Debug.LogError("Material not created.");
                return;
            }

            var cameraData = frameData.Get<UniversalCameraData>();
            if (!cameraData.postProcessEnabled) return;

            var stack = VolumeManager.instance.stack;
            this.dithering = stack.GetComponent<Dithering>();
            if (this.dithering == null) return;
            if (!this.dithering.IsActive()) return;

            this.ditheringMaterial.SetInt(PatternIndex, this.dithering.patternIndex.value);
            this.ditheringMaterial.SetFloat(DitherThreshold, this.dithering.ditherThreshold.value);
            this.ditheringMaterial.SetFloat(DitherStrength, this.dithering.ditherStrength.value);
            this.ditheringMaterial.SetFloat(DitherScale, this.dithering.ditherScale.value);

            var resourceData = frameData.Get<UniversalResourceData>();
            if (resourceData.isActiveTargetBackBuffer) return;

            var source = resourceData.activeColorTexture;

            var destinationDesc = renderGraph.GetTextureDesc(source);
            destinationDesc.name = "_TempTargetDithering";
            destinationDesc.clearBuffer = false;
            TextureHandle destination = renderGraph.CreateTexture(destinationDesc);

            RenderGraphUtils.BlitMaterialParameters blitParams = new(source, destination, this.ditheringMaterial, 0);
            renderGraph.AddBlitPass(blitParams, k_RenderTag);

            resourceData.cameraColor = destination;
        }

        public void Dispose()
        {
            CoreUtils.Destroy(ditheringMaterial);
        }
    }
}