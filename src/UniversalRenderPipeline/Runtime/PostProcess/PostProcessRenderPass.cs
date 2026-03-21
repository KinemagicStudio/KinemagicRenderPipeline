using System;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace Kinemagic.Rendering.Universal
{
    public abstract class PostProcessRenderPass : ScriptableRenderPass, IDisposable
    {
        public abstract PostProcessComponent Component { get; set; }
        public abstract void SetTextures(TextureHandle source, TextureHandle destination);
        public abstract void Dispose();
    }
}
