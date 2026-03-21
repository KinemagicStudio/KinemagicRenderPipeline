using UnityEngine;

namespace Kinemagic.Rendering.Universal
{
    [RequireComponent(typeof(Camera))]
    public class KinemagicCameraData : MonoBehaviour
    {
        [SerializeField] private PostProcessStackData _defaultPostProcessStackData;

        private IPostProcessStack _postProcessStack;

        public IPostProcessStack PostProcessStack
        {
            get => _postProcessStack ?? _defaultPostProcessStackData;
            set => _postProcessStack = value;
        }
    }
}