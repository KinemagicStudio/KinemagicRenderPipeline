using System.Collections.Generic;
using UnityEngine;

namespace Kinemagic.Rendering.Universal
{
    [CreateAssetMenu(menuName = "Kinemagic/PostProcessStackData")]
    [System.Serializable]
    public sealed class PostProcessStackData : ScriptableObject, IPostProcessStack
    {
        [SerializeField] private List<PostProcessComponent> _components = new();

        public string Name => this.name;
        public long CreatedAt { get; set; } = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        public IReadOnlyList<PostProcessComponent> GetComponents() => _components;

        public bool IsActive()
        {
            var hasActiveComponent = false;

            foreach (var component in _components)
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
