using UnityEngine;

namespace BoatGame.Boat
{
    [RequireComponent(typeof(BoatView))]
    public class BoatWake : MonoBehaviour
    {
        [SerializeField] private Material wakeMaterial;
        [SerializeField] private Vector3 sternOffset = new Vector3(0f, 0.05f, -1.5f);
        [SerializeField] private float width = 1.2f;
        [SerializeField] private float lifetime = 4f;
        [SerializeField] private float minVertexDistance = 0.2f;

        private TrailRenderer _trail;

        private void Start()
        {
            var anchor = new GameObject("Wake");
            anchor.transform.SetParent(transform, false);
            anchor.transform.localPosition = sternOffset;
            anchor.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

            var trail = _trail = anchor.AddComponent<TrailRenderer>();
            trail.sharedMaterial = wakeMaterial;
            trail.alignment = LineAlignment.TransformZ;
            trail.time = lifetime;
            trail.minVertexDistance = minVertexDistance;
            trail.textureMode = LineTextureMode.Stretch;
            trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            trail.receiveShadows = false;
            trail.widthCurve = new AnimationCurve(new Keyframe(0f, width), new Keyframe(1f, width * 2.5f));
            trail.colorGradient = new Gradient
            {
                alphaKeys = new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) },
                colorKeys = new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) }
            };
        }
        
        public void ShiftWake(Vector2 shift)
        {
            if (_trail.positionCount == 0) return;

            var positions = new Vector3[_trail.positionCount];
            _trail.GetPositions(positions);
            var offset = new Vector3(shift.x, 0f, shift.y);
            for (var i = 0; i < positions.Length; i++) positions[i] += offset;
            _trail.SetPositions(positions);
        }
    }
}