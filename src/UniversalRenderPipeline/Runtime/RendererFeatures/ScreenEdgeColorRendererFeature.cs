using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Kinemagic.Rendering.Universal
{
    public sealed class ScreenEdgeColorRendererFeature : ScriptableRendererFeature
    {
        public InjectionPoint InjectionPoint = InjectionPoint.AfterRenderingPostProcessing;

        private ScreenEdgeColorRenderPass _renderPass;

        public override void Create()
        {
            _renderPass = new ScreenEdgeColorRenderPass();
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType == CameraType.Preview ||
                renderingData.cameraData.cameraType == CameraType.SceneView ||
                UniversalRenderer.IsOffscreenDepthTexture(ref renderingData.cameraData))
            {
                return;
            }

            _renderPass.renderPassEvent = (RenderPassEvent)InjectionPoint;
            renderer.EnqueuePass(_renderPass);
        }

        protected override void Dispose(bool disposing)
        {
            _renderPass?.Dispose();
        }
    }
}
