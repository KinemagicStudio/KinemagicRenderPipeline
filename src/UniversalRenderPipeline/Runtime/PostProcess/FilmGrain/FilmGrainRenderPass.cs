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

            var offset = CalcGrainOffset(filmGrain);
            _material.SetVector(GrainTextureParamsPropId, new Vector4(
                width / grainTexture.width,
                height / grainTexture.height,
                offset.x,
                offset.y
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

        private static Vector2 CalcGrainOffset(FilmGrain filmGrain)
        {
            int grainFrameIndex;

            if (filmGrain.OverrideFrameIndex >= 0)
            {
                grainFrameIndex = filmGrain.OverrideFrameIndex;
            }
            else if (filmGrain.FrameRate > 0f)
            {
                grainFrameIndex = Mathf.FloorToInt(Time.time * filmGrain.FrameRate);
            }
            else
            {
                grainFrameIndex = Time.frameCount;
            }

            return DeterministicOffset(filmGrain.Seed + grainFrameIndex);
        }

        private static Vector2 DeterministicOffset(int seed)
        {
            var oldState = Random.state;
            Random.InitState(seed);
            var offset = new Vector2(Random.value, Random.value);
            Random.state = oldState;
            return offset;
        }
    }
}
