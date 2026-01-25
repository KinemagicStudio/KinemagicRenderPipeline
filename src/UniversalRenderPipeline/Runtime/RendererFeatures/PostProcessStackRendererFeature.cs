using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace Kinemagic.Rendering.Universal
{
    /// <summary>
    /// The class for the post-process stack renderer feature.
    /// </summary>
    [DisallowMultipleRendererFeature("Post Process Stack")]
    public sealed class PostProcessStackRendererFeature : ScriptableRendererFeature
    {
        private PostProcessStackRenderGraphRecorder _renderGraphRecorder;

        public override void Create()
        {
            if (_renderGraphRecorder == null)
            {
                _renderGraphRecorder = new PostProcessStackRenderGraphRecorder();
            }
        }

        protected override void Dispose(bool disposing)
        {
            _renderGraphRecorder?.Dispose();
            _renderGraphRecorder = null;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType == CameraType.SceneView ||
                renderingData.cameraData.cameraType == CameraType.Preview ||
                renderingData.cameraData.cameraType == CameraType.Reflection ||
                UniversalRenderer.IsOffscreenDepthTexture(ref renderingData.cameraData))
            {
                return;
            }

            renderer.EnqueuePass(_renderGraphRecorder);
        }

        private sealed class PostProcessStackRenderGraphRecorder : ScriptableRenderPass, IDisposable
        {
            private class BlitPassData : PostProcessPassData
            {
            }

            private static readonly RenderPassEvent RenderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;

            private readonly PostProcessRenderPassFactoryProvider _renderPassFactoryProvider;
            private readonly List<PostProcessRenderPass> _renderPasses = new();
            private readonly List<PostProcessComponent> _components = new();

            private IPostProcessStack _stack;

            public PostProcessStackRenderGraphRecorder()
            {
                renderPassEvent = RenderPassEvent;
                _renderPassFactoryProvider = PostProcessRenderPassFactoryProvider.Instance;
            }

            public void Dispose()
            {
                foreach (var renderPass in _renderPasses)
                {
                    renderPass.Dispose();
                }
                _renderPasses.Clear();
                _components.Clear();
            }

            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                var resourceData = frameData.Get<UniversalResourceData>();
                var universalCameraData = frameData.Get<UniversalCameraData>();

                if (!universalCameraData.camera.TryGetComponent<KinemagicCameraData>(out var cameraData))
                {
                    return;
                }

                _stack = cameraData.PostProcessStack;
                if (_stack == null || !_stack.IsActive())
                {
                    return;
                }

                CreateRenderPassList();

                var descriptor = universalCameraData.cameraTargetDescriptor;
                descriptor.depthStencilFormat = GraphicsFormat.None;
                descriptor.msaaSamples = 1;

                var tempRT0 = UniversalRenderer.CreateRenderGraphTexture(
                    renderGraph, descriptor, "_CustomPostProcessTexture0", true, FilterMode.Bilinear);
                var tempRT1 = UniversalRenderer.CreateRenderGraphTexture(
                    renderGraph, descriptor, "_CustomPostProcessTexture1", true, FilterMode.Bilinear);

                using (var builder = renderGraph.AddUnsafePass<BlitPassData>("PostProcessStack_Init", out var passData))
                {
                    builder.UseTexture(resourceData.cameraColor, AccessFlags.Read);

                    // Create render textures
                    builder.UseTexture(tempRT0, AccessFlags.Write);
                    builder.UseTexture(tempRT1, AccessFlags.Write);
                    builder.SetGlobalTextureAfterPass(tempRT0, Shader.PropertyToID("_CustomPostProcessTexture0"));
                    builder.SetGlobalTextureAfterPass(tempRT1, Shader.PropertyToID("_CustomPostProcessTexture1"));

                    passData.SourceTexture = resourceData.cameraColor;
                    passData.DestinationTexture = tempRT0;

                    builder.SetRenderFunc((BlitPassData data, UnsafeGraphContext context) =>
                    {
                        var cmd = CommandBufferHelpers.GetNativeCommandBuffer(context.cmd);
                        Blitter.BlitCameraTexture(cmd, data.SourceTexture, data.DestinationTexture);
                    });
                }

                var source = resourceData.cameraColor;
                var destination = tempRT0;
                var nextDestination = tempRT1;
                for (var index = 0; index < _renderPasses.Count; index++)
                {
                    var current = _components[index];
                    if (!current.IsActive())
                    {
                        continue;
                    }

                    // Swap texture handles
                    if (index > 0)
                    {
                        source = destination;
                        (destination, nextDestination) = (nextDestination, destination);
                    }

                    var renderPass = _renderPasses[index];
                    renderPass.Component = _components[index];
                    renderPass.SetTextures(source, destination);
                    renderPass.RecordRenderGraph(renderGraph, frameData);
                }

                resourceData.cameraColor = destination;
            }

            private void CreateRenderPassList()
            {
                foreach (var renderPass in _renderPasses)
                {
                    renderPass.Dispose();
                }
                _renderPasses.Clear();
                _components.Clear();

                var components = _stack.GetComponents();
                for (var i = 0; i < components.Count; i++)
                {
                    var component = components[i];
                    if (component != null)
                    {
                        if (_renderPassFactoryProvider.TryGetFactory(component.GetType(), out var factory))
                        {
                            var renderPass = factory.Invoke(component);
                            if (renderPass != null)
                            {
                                _renderPasses.Add(renderPass);
                                _components.Add(component);
                            }
                        }
                    }
                }

                foreach (var renderPass in _renderPasses)
                {
                    renderPass.renderPassEvent = RenderPassEvent;
                }
            }
        }
    }
}
