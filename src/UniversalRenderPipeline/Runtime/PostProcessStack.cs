using System;
using System.Collections.Generic;

namespace Kinemagic.Rendering.Universal
{
    public interface IPostProcessStack
    {
        string Name { get; }
        long CreatedAt { get; }
        IReadOnlyList<PostProcessComponent> GetComponents();
        bool IsActive();
    }

    [Serializable]
    public sealed class PostProcessStack : IPostProcessStack
    {
        public string Name { get; set; }
        public long CreatedAt { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        public List<PostProcessComponent> Components { get; set; }

        public IReadOnlyList<PostProcessComponent> GetComponents() => Components;

        public bool IsActive()
        {
            var hasActiveComponent = false;

            foreach (var component in Components)
            {
                if (component.IsActive())
                {
                    hasActiveComponent = true;
                    break;
                }
            }

            return hasActiveComponent;
        }
    }
}