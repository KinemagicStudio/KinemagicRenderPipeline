using UnityEngine.Rendering.Universal;

namespace Kinemagic.Rendering.Universal
{
    /// <summary>
    /// An injection point for the full screen pass. This is similar to the RenderPassEvent enum but limited to only supported events.
    /// </summary>
    public enum InjectionPoint
    {
        /// <summary>
        /// Inject a full screen pass before transparents are rendered.
        /// </summary>
        BeforeRenderingTransparents = RenderPassEvent.BeforeRenderingTransparents,

        /// <summary>
        /// Inject a full screen pass before post processing is rendered.
        /// </summary>
        BeforeRenderingPostProcessing = RenderPassEvent.BeforeRenderingPostProcessing,

        /// <summary>
        /// Inject a full screen pass after post processing is rendered.
        /// </summary>
        AfterRenderingPostProcessing = RenderPassEvent.AfterRenderingPostProcessing
    }
}