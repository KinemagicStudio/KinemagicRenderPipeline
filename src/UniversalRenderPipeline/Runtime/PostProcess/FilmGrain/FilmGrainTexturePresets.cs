using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kinemagic.Rendering.Universal
{
    [CreateAssetMenu(menuName = "Kinemagic/PostProcessStackSettings/FilmGrainTexturePresets")]
    public sealed class FilmGrainTexturePresets : ScriptableObject
    {
        private static readonly string[] PresetTexturePaths =
        {
            "Packages/com.unity.render-pipelines.universal/Textures/FilmGrain/Thin01.png",
            "Packages/com.unity.render-pipelines.universal/Textures/FilmGrain/Thin02.png",
            "Packages/com.unity.render-pipelines.universal/Textures/FilmGrain/Medium01.png",
            "Packages/com.unity.render-pipelines.universal/Textures/FilmGrain/Medium02.png",
            "Packages/com.unity.render-pipelines.universal/Textures/FilmGrain/Medium03.png",
            "Packages/com.unity.render-pipelines.universal/Textures/FilmGrain/Medium04.png",
            "Packages/com.unity.render-pipelines.universal/Textures/FilmGrain/Medium05.png",
            "Packages/com.unity.render-pipelines.universal/Textures/FilmGrain/Medium06.png",
            "Packages/com.unity.render-pipelines.universal/Textures/FilmGrain/Large01.png",
            "Packages/com.unity.render-pipelines.universal/Textures/FilmGrain/Large02.png"
        };

        public Texture2D Thin01;
        public Texture2D Thin02;
        public Texture2D Medium01;
        public Texture2D Medium02;
        public Texture2D Medium03;
        public Texture2D Medium04;
        public Texture2D Medium05;
        public Texture2D Medium06;
        public Texture2D Large01;
        public Texture2D Large02;

        public Texture2D GetTexture(FilmGrainLookup type)
        {
            return type switch
            {
                FilmGrainLookup.Thin1 => Thin01,
                FilmGrainLookup.Thin2 => Thin02,
                FilmGrainLookup.Medium1 => Medium01,
                FilmGrainLookup.Medium2 => Medium02,
                FilmGrainLookup.Medium3 => Medium03,
                FilmGrainLookup.Medium4 => Medium04,
                FilmGrainLookup.Medium5 => Medium05,
                FilmGrainLookup.Medium6 => Medium06,
                FilmGrainLookup.Large01 => Large01,
                FilmGrainLookup.Large02 => Large02,
                _ => null
            };
        }

#if UNITY_EDITOR
        private void Reset()
        {
            LoadTextures();
        }

        private void OnValidate()
        {
            LoadTextures();
        }

        private void LoadTextures()
        {
            Thin01 = AssetDatabase.LoadAssetAtPath<Texture2D>(PresetTexturePaths[(int)FilmGrainLookup.Thin1]);
            Thin02 = AssetDatabase.LoadAssetAtPath<Texture2D>(PresetTexturePaths[(int)FilmGrainLookup.Thin2]);
            Medium01 = AssetDatabase.LoadAssetAtPath<Texture2D>(PresetTexturePaths[(int)FilmGrainLookup.Medium1]);
            Medium02 = AssetDatabase.LoadAssetAtPath<Texture2D>(PresetTexturePaths[(int)FilmGrainLookup.Medium2]);
            Medium03 = AssetDatabase.LoadAssetAtPath<Texture2D>(PresetTexturePaths[(int)FilmGrainLookup.Medium3]);
            Medium04 = AssetDatabase.LoadAssetAtPath<Texture2D>(PresetTexturePaths[(int)FilmGrainLookup.Medium4]);
            Medium05 = AssetDatabase.LoadAssetAtPath<Texture2D>(PresetTexturePaths[(int)FilmGrainLookup.Medium5]);
            Medium06 = AssetDatabase.LoadAssetAtPath<Texture2D>(PresetTexturePaths[(int)FilmGrainLookup.Medium6]);
            Large01 = AssetDatabase.LoadAssetAtPath<Texture2D>(PresetTexturePaths[(int)FilmGrainLookup.Large01]);
            Large02 = AssetDatabase.LoadAssetAtPath<Texture2D>(PresetTexturePaths[(int)FilmGrainLookup.Large02]);
            EditorUtility.SetDirty(this);
        }
#endif
    }
}
