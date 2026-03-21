using System.Collections.Generic;
using UnityEngine;

namespace Kinemagic.Rendering.Universal
{
    public static class FilmGrainTextureProvider
    {
        private static FilmGrainTexturePresets _presets;
        private static readonly Dictionary<string, Texture2D> _customTextures = new();

        public static void Clear()
        {
            _presets = null;
            _customTextures.Clear();
        }

        public static void RegisterPresets(FilmGrainTexturePresets presets)
        {
            _presets = presets;
        }

        public static void RegisterCustomTexture(string key, Texture2D texture)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }
            _customTextures[key] = texture;
        }

        public static void UnregisterCustomTexture(string key)
        {
            if (string.IsNullOrEmpty(key)) 
            {
                return;
            }
            _customTextures.Remove(key);
        }

        public static Texture2D GetPresetTexture(FilmGrainLookup type)
        {
            if (type == FilmGrainLookup.Custom)
            {
                return null;
            }
            return _presets != null ? _presets.GetTexture(type) : null;
        }

        public static Texture2D GetCustomTexture(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;                
            }
            return _customTextures.TryGetValue(key, out var texture) ? texture : null;
        }

        public static bool HasPresets => _presets != null;

        public static bool HasCustomTexture(string key)
        {
            return !string.IsNullOrEmpty(key) && _customTextures.ContainsKey(key);
        }
    }
}
