using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

namespace Kinemagic.Rendering.Universal
{
    public sealed class ScreenEdgeColorRenderPass : ScriptableRenderPass, IDisposable
    {
        private const string ShaderName = "Kinemagic/Universal/ScreenEdgeColor";
        private const string RenderPassName = "ScreenEdgeColor";
        private static readonly int IntensityPropId = Shader.PropertyToID("_Intensity");
        private static readonly int ColorArrayPropId = Shader.PropertyToID("_ColorArray");

        private Material _material;
        private Color[] _edgeColors = new Color[4];

        private class PassData
        {
            public TextureHandle Source;
            public Material BlitMaterial;
            public int PassIndex;
        }

        public ScreenEdgeColorRenderPass()
        {
            renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
            profilingSampler = new ProfilingSampler(RenderPassName);
        }

        public void Dispose()
        {
            if (_material != null)
            {
                CoreUtils.Destroy(_material);
                _material = null;
            }
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var cameraData = frameData.Get<UniversalCameraData>();
            if (!cameraData.postProcessEnabled)
            {
                return;
            }

            var settings = VolumeManager.instance.stack.GetComponent<ScreenEdgeColor>();
            if (settings == null || !settings.IsActive())
            {
                return;
            }

            _edgeColors[0] = settings.TopLeftColor;
            _edgeColors[1] = settings.TopRightColor;
            _edgeColors[2] = settings.BottomLeftColor;
            _edgeColors[3] = settings.BottomRightColor;

            if (_material == null)
            {
                _material = CoreUtils.CreateEngineMaterial(ShaderName);
            }
            _material.SetFloat(IntensityPropId, settings.Intensity);
            _material.SetColorArray(ColorArrayPropId, _edgeColors);

            var resourceData = frameData.Get<UniversalResourceData>();
            var cameraColor = resourceData.cameraColor;

            var targetTextureDesc = renderGraph.GetTextureDesc(resourceData.cameraColor);
            var targetTextureHandle = renderGraph.CreateTexture(targetTextureDesc);

            // Add pass
            using (var builder = renderGraph.AddRasterRenderPass<PassData>(RenderPassName, out var passData, profilingSampler))
            {
                passData.Source = cameraColor;
                passData.BlitMaterial = _material;
                passData.PassIndex = 0;

                builder.UseTexture(cameraColor, AccessFlags.Read);
                builder.SetRenderAttachment(targetTextureHandle, 0, AccessFlags.Write);

                builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                {
                    Blitter.BlitTexture(context.cmd, data.Source, Vector2.one, data.BlitMaterial, data.PassIndex);
                });
            }

            // Update the active color texture
            resourceData.cameraColor = targetTextureHandle;
        }
    }
}
