using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

namespace PSX
{
    public class PixelationRenderFeature : ScriptableRendererFeature
    {
        PixelationPass pixelationPass;

        public override void Create()
        {
            pixelationPass = new PixelationPass(RenderPassEvent.BeforeRenderingPostProcessing);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(pixelationPass);
        }

        protected override void Dispose(bool disposing)
        {
            pixelationPass?.Dispose();
        }
    }


    public class PixelationPass : ScriptableRenderPass
    {
        private static readonly string shaderPath = "PostEffect/Pixelation";
        static readonly string k_RenderTag = "Render Pixelation Effects";
        static readonly int WidthPixelation = Shader.PropertyToID("_WidthPixelation");
        static readonly int HeightPixelation = Shader.PropertyToID("_HeightPixelation");
        static readonly int ColorPrecision = Shader.PropertyToID("_ColorPrecision");

        Pixelation pixelation;
        Material pixelationMaterial;

        public PixelationPass(RenderPassEvent evt)
        {
            renderPassEvent = evt;
            profilingSampler = new ProfilingSampler(k_RenderTag);

            var shader = Shader.Find(shaderPath);
            if (shader == null)
            {
                Debug.LogError("Shader not found.");
                return;
            }
            this.pixelationMaterial = CoreUtils.CreateEngineMaterial(shader);
        }

        // Replaces the old Execute(ScriptableRenderContext, ref RenderingData) entry point.
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            if (this.pixelationMaterial == null)
            {
                Debug.LogError("Material not created.");
                return;
            }

            var cameraData = frameData.Get<UniversalCameraData>();
            if (!cameraData.postProcessEnabled) return;

            var stack = VolumeManager.instance.stack;
            this.pixelation = stack.GetComponent<Pixelation>();
            if (this.pixelation == null) return;
            if (!this.pixelation.IsActive()) return;

            this.pixelationMaterial.SetFloat(WidthPixelation, this.pixelation.widthPixelation.value);
            this.pixelationMaterial.SetFloat(HeightPixelation, this.pixelation.heightPixelation.value);
            this.pixelationMaterial.SetFloat(ColorPrecision, this.pixelation.colorPrecision.value);

            var resourceData = frameData.Get<UniversalResourceData>();
            if (resourceData.isActiveTargetBackBuffer) return;

            var source = resourceData.activeColorTexture;

            var destinationDesc = renderGraph.GetTextureDesc(source);
            destinationDesc.name = "_TempTargetPixelation";
            destinationDesc.clearBuffer = false;
            TextureHandle destination = renderGraph.CreateTexture(destinationDesc);

            RenderGraphUtils.BlitMaterialParameters blitParams = new(source, destination, this.pixelationMaterial, 0);
            renderGraph.AddBlitPass(blitParams, k_RenderTag);

            resourceData.cameraColor = destination;
        }

        public void Dispose()
        {
            CoreUtils.Destroy(pixelationMaterial);
        }
    }
}