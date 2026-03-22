using UnityEngine;

namespace Kinemagic.Rendering.Universal
{
    [CreateAssetMenu(menuName = "Kinemagic/PostProcessStackSettings/PostProcessStackSettings")]
    public sealed class PostProcessStackSettings : ScriptableObject
    {
        [Header("Film Grain")]
        [Tooltip("Preset textures for FilmGrain effect.")]
        public FilmGrainTexturePresets FilmGrainTexturePresets;

        public void ApplySettings()
        {
            if (FilmGrainTexturePresets != null)
            {
                FilmGrainTextureProvider.RegisterPresets(FilmGrainTexturePresets);
            }
        }
    }
}
