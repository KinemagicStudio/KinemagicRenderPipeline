using UnityEngine;

namespace Kinemagic.Rendering.Universal
{
    [CreateAssetMenu(menuName = "Kinemagic/PostProcess/FilmGrain")]
    [System.Serializable]
    public sealed class FilmGrain : PostProcessComponent
    {
        [Header("Film Grain")]
        [Tooltip("The type of grain to use. Select a preset or choose Custom to use your own texture.")]
        public FilmGrainLookup Type = FilmGrainLookup.Thin1;

        [Tooltip("The strength of the film grain effect.")]
        [Range(0f, 1f)]
        public float Intensity = 0f;

        [Tooltip("Controls the noisiness response curve based on scene luminance. Higher values mean less noise in light areas.")]
        [Range(0f, 1f)]
        public float Response = 0.8f;

        [Header("Custom Texture")]
        [Tooltip("Key to identify the custom texture registered in FilmGrainTextureProvider. Only used when Type is set to Custom.")]
        public string CustomTextureKey;

        public override string GetName() => "FilmGrain";

        public override bool IsActive()
        {
            if (Intensity <= 0f)
            {
                return false;
            }
            return GetActiveTexture() != null;
        }

        public Texture2D GetActiveTexture()
        {
            if (Type == FilmGrainLookup.Custom)
            {
                return FilmGrainTextureProvider.GetCustomTexture(CustomTextureKey);
            }
            return FilmGrainTextureProvider.GetPresetTexture(Type);
        }
    }
}
