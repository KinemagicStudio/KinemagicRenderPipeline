using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

namespace Kinemagic.Rendering.Universal
{
    public sealed class FilmGrainPassData : PostProcessPassData
    {
        public Material BlitMaterial;
        public int PassIndex;
    }

    public sealed class FilmGrainRenderPass : PostProcessRenderPass
    {
        private const string ShaderName = "Kinemagic/Universal/FilmGrain";
        private const string RenderPassName = "FilmGrain";

        private static readonly int GrainTexturePropId = Shader.PropertyToID("_GrainTexture");
        private static readonly int GrainParamsPropId = Shader.PropertyToID("_GrainParams");
        private static readonly int GrainTextureParamsPropId = Shader.PropertyToID("_GrainTextureParams");

        private TextureHandle _source;
        private TextureHandle _output;
        private Material _material;

        public override PostProcessComponent Component { get; set; }

        public FilmGrainRenderPass()
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
            if (Component is not FilmGrain filmGrain || !filmGrain.IsActive())
            {
                return;
            }

            var grainTexture = filmGrain.GetActiveTexture();
            if (grainTexture == null)
            {
                return;
            }

            var cameraData = frameData.Get<UniversalCameraData>();

            // Set grain texture
            _material.SetTexture(GrainTexturePropId, grainTexture);

            // Calculate texture params for proper tiling/offset animation
            float width = cameraData.cameraTargetDescriptor.width;
            float height = cameraData.cameraTargetDescriptor.height;

            _material.SetVector(GrainTextureParamsPropId, new Vector4(
                width / grainTexture.width,
                height / grainTexture.height,
                Random.value,
                Random.value
            ));

            // Set grain parameters (intensity scaled by 4 to match URP behavior)
            _material.SetVector(GrainParamsPropId, new Vector3(
                filmGrain.Intensity * 4f,
                filmGrain.Response,
                0f
            ));

            var resourceData = frameData.Get<UniversalResourceData>();
            var source = _source.IsValid() ? _source : resourceData.cameraColor;
            var destination = _output.IsValid() ? _output : renderGraph.CreateTexture(source, "_FilmGrainPassTexture");

            using (var builder = renderGraph.AddRasterRenderPass<FilmGrainPassData>(RenderPassName, out var passData))
            {
                passData.SourceTexture = source;
                passData.DestinationTexture = destination;
                passData.BlitMaterial = _material;
                passData.PassIndex = 0;

                builder.UseTexture(passData.SourceTexture, AccessFlags.Read);
                builder.SetRenderAttachment(passData.DestinationTexture, 0, AccessFlags.Write);

                builder.SetRenderFunc((FilmGrainPassData data, RasterGraphContext context) =>
                {
                    Blitter.BlitTexture(context.cmd, data.SourceTexture, Vector2.one, data.BlitMaterial, data.PassIndex);
                });
            }

            if (!_output.IsValid())
            {
                resourceData.cameraColor = destination;
            }
        }
    }
}
