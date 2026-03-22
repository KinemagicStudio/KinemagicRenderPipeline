using UnityEngine;

namespace Kinemagic.Rendering.Universal
{
    public static class KinemagicRenderPipelineInitializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RegisterRenderPassFactories()
        {
            DebugLogger.Log("<color=lime>[KinemagicRenderPipelineInitializer] Register render pass factories.</color>");

            var factoryProvider = PostProcessRenderPassFactoryProvider.Instance;

            factoryProvider.Register<ScreenEdgeColor>(_ => new ScreenEdgeColorRenderPass());
            factoryProvider.Register<FilmGrain>(_ => new FilmGrainRenderPass());
        }
    }
}
