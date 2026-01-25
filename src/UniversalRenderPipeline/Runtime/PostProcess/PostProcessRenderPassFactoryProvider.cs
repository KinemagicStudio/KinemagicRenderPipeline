using System;
using System.Collections.Generic;

namespace Kinemagic.Rendering.Universal
{
    public sealed class PostProcessRenderPassFactoryProvider
    {
        private static readonly Lazy<PostProcessRenderPassFactoryProvider> _instance = new (() => new PostProcessRenderPassFactoryProvider());
        public static PostProcessRenderPassFactoryProvider Instance => _instance.Value;

        private readonly Dictionary<Type, Func<PostProcessComponent, PostProcessRenderPass>> _factories = new();

        public void Register<T>(Func<PostProcessComponent, PostProcessRenderPass> factory) where T : PostProcessComponent
        {
            _factories[typeof(T)] = factory;
        }

        public bool TryGetFactory(Type type, out Func<PostProcessComponent, PostProcessRenderPass> factory)
        {
            return _factories.TryGetValue(type, out factory);
        }
    }
}
