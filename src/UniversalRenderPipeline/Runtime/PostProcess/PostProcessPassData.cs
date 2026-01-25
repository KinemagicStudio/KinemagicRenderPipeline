using UnityEngine.Rendering.RenderGraphModule;

namespace Kinemagic.Rendering.Universal
{
    public abstract class PostProcessPassData
    {
        public TextureHandle SourceTexture;
        public TextureHandle DestinationTexture;
    }
}
