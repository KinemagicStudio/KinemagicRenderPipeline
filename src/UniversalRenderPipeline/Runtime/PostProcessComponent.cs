using UnityEngine;

namespace Kinemagic.Rendering.Universal
{
    public abstract class PostProcessComponent : ScriptableObject
    {
        public abstract string GetName();
        public abstract bool IsActive();
    }
}