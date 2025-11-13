using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace Kinemagic.Rendering.Universal
{
    [VolumeComponentMenu("Kinemagic/Post-processing/ScreenEdgeColor")]
    public sealed class ScreenEdgeColor : VolumeComponent, IPostProcessComponent
    {
        [FormerlySerializedAs("Intensity")]
        public ClampedFloatParameter IntensityParam = new ClampedFloatParameter(0.1f, 0f, 1f, overrideState: true);

        [FormerlySerializedAs("TopLeftColor")]
        public ColorParameter TopLeftColorParam = new ColorParameter(Color.cyan, overrideState: true);

        [FormerlySerializedAs("TopRightColor")]
        public ColorParameter TopRightColorParam = new ColorParameter(Color.magenta, overrideState: true);

        [FormerlySerializedAs("BottomLeftColor")]
        public ColorParameter BottomLeftColorParam = new ColorParameter(Color.yellow, overrideState: true);

        [FormerlySerializedAs("BottomRightColor")]
        public ColorParameter BottomRightColorParam = new ColorParameter(Color.red, overrideState: true);

        public float Intensity => IntensityParam.value;
        public Color TopLeftColor => TopLeftColorParam.value;
        public Color TopRightColor => TopRightColorParam.value;
        public Color BottomLeftColor => BottomLeftColorParam.value;
        public Color BottomRightColor => BottomRightColorParam.value;

        public bool IsActive() => Intensity > 0f;
    }
}