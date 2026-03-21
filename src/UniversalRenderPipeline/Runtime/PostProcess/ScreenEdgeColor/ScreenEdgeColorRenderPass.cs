using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

namespace Kinemagic.Rendering.Universal
{
    public sealed class ScreenEdgeColorPassData : PostProcessPassData
    {
        public Material BlitMaterial;
        public int PassIndex;
    }

    public sealed class ScreenEdgeColorRenderPass : PostProcessRenderPass
    {
        private const string ShaderName = "Kinemagic/Universal/ScreenEdgeColor";
        private const string RenderPassName = "ScreenEdgeColor";
        private static readonly int IntensityPropId = Shader.PropertyToID("_Intensity");
        private static readonly int ColorArrayPropId = Shader.PropertyToID("_ColorArray");
        private readonly Color[] _edgeColors = new Color[4];

        private TextureHandle _source;
        private TextureHandle _output;
        private Material _material;

        public override PostProcessComponent Component { get; set; }

        public ScreenEdgeColorRenderPass()
        {
            _material = CoreUtils.CreateEngineMaterial(ShaderName);
        }

        public override void Dispose()
        {
            if (_material != null)
            {
                CoreUtils.Destroy(_material);
                _material = null;
            }
        }

        public override void SetTextures(TextureHandle source, TextureHandle output)
        {
            _source = source;
            _output = output;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            if (Component == null || !Component.IsActive())
            {
                return;
            }

            var screenEdgeColor = Component as ScreenEdgeColor;
            if (screenEdgeColor == null)
            {
                return;
            }

            _edgeColors[0] = screenEdgeColor.TopLeftColor;
            _edgeColors[1] = screenEdgeColor.TopRightColor;
            _edgeColors[2] = screenEdgeColor.BottomLeftColor;
            _edgeColors[3] = screenEdgeColor.BottomRightColor;

            _material.SetFloat(IntensityPropId, screenEdgeColor.Intensity);
            _material.SetColorArray(ColorArrayPropId, _edgeColors);

            var renderPassName = Component.GetName();
            if (string.IsNullOrEmpty(renderPassName)) renderPassName = RenderPassName;

            using (var builder = renderGraph.AddRasterRenderPass<ScreenEdgeColorPassData>(renderPassName, out var passData))
            {
                passData.SourceTexture = _source;
                passData.DestinationTexture = _output;
                passData.BlitMaterial = _material;
                passData.PassIndex = 0;

                builder.UseTexture(passData.SourceTexture, AccessFlags.Read);
                builder.SetRenderAttachment(passData.DestinationTexture, 0, AccessFlags.Write);

                builder.SetRenderFunc((ScreenEdgeColorPassData data, RasterGraphContext context) =>
                {
                    Blitter.BlitTexture(context.cmd, data.SourceTexture, Vector2.one, data.BlitMaterial, data.PassIndex);
                });
            }
        }
    }
}
