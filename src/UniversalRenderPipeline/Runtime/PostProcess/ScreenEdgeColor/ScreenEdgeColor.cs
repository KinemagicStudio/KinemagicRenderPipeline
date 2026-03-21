using UnityEngine;

namespace Kinemagic.Rendering.Universal
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Kinemagic/PostProcess/ScreenEdgeColor")]
    public sealed class ScreenEdgeColor : PostProcessComponent
    {
        public bool IsEnabled { get; set; } = true;
        public string PassName = "ScreenEdgeColor";

        [Range(0f, 1f)] public float Intensity = 0f;
        public Color TopLeftColor = Color.cyan;
        public Color TopRightColor = Color.magenta;
        public Color BottomLeftColor = Color.yellow;
        public Color BottomRightColor = Color.red;

        public override string GetName() => PassName;
        public override bool IsActive() => Intensity > 0f && IsEnabled;
    }
}