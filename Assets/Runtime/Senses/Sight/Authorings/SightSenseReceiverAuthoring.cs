using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
#if UNITY_EDITOR
using ECSPerception.Editor;
using UnityEditor;
#endif

namespace ECSPerception.Sight
{
    public sealed class SightSenseReceiverAuthoring : MonoBehaviour
    {
        [SerializeField] private float _viewRadius = 5;
        [SerializeField] private float _loseRadius = 7;
        [SerializeField, Min(0)] private float _nearClipRadius = 0.5f;
        [SerializeField] private float3 _offset = float3.zero;
        [SerializeField, Range(0, 360)] private float _viewAngleDegrees = 90;
        [SerializeField, Min(0)] private float _rememberTime = 1;

        [Header("Debug Options")]
        [SerializeField] private Color _colorNearClip = Color.gray;
        [SerializeField] private Color _colorView = Color.green;
        [SerializeField] private Color _colorLose = Color.red;
        [SerializeField, Min(0)] private float _curveStep = 0.02f;

        private sealed class Baker : Baker<SightSenseReceiverAuthoring>
        {
            public override void Bake(SightSenseReceiverAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ComponentSenseSightReceiver
                {
                    ViewAngleCos = math.cos(math.radians(authoring._viewAngleDegrees / 2)),
                    ViewRadiusSquared = math.pow(authoring._viewRadius, 2),
                    LoseRadiusSquared = math.pow(authoring._loseRadius, 2),
                    NearClipRadiusSquared = math.pow(authoring._nearClipRadius, 2),
                    Offset = authoring._offset,
                    RememberTime = authoring._rememberTime,
                });
                AddComponent<TagSenseSightMultiCast>(entity);
                AddBuffer<BufferSenseSightCastMultiOffset>(entity);
                AddBuffer<BufferSenseSightActive>(entity);
                AddBuffer<BufferSenseSightCastNeed>(entity);
                AddBuffer<BufferSenseSightCastPending>(entity);
                AddBuffer<BufferSenseSightRemember>(entity);
                SetComponentEnabled<BufferSenseSightCastNeed>(entity, false);
                SetComponentEnabled<BufferSenseSightCastPending>(entity, false);

#if UNITY_EDITOR
                AddComponent<TagSenseDebug>(entity);
                SetComponentEnabled<TagSenseDebug>(entity, false);
#endif
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            _viewRadius = math.max(_viewRadius, _nearClipRadius);
            _loseRadius = math.max(_loseRadius, _viewRadius);
        }

        private void OnDrawGizmosSelected()
        {
            if (EditorApplication.isPlaying)
            {
                return;
            }

            var forward = transform.forward;
            var right = transform.right;
            var angleRadians = math.radians(_viewAngleDegrees);

            var x = math.cos(angleRadians / 2);
            var y = math.sin(angleRadians / 2);

            var startPoint = transform.TransformPoint(_offset);

            if (_viewAngleDegrees != 360)
            {
                var nearClipPoint1 = startPoint + forward * x * _nearClipRadius + right * y * _nearClipRadius;
                var nearClipPoint2 = startPoint + forward * x * _nearClipRadius + right * -y * _nearClipRadius;
                var viewPoint1 = startPoint + forward * x * _viewRadius + right * y * _viewRadius;
                var viewPoint2 = startPoint + forward * x * _viewRadius + right * y * -_viewRadius;
                var losePoint1 = startPoint + forward * x * _loseRadius + right * y * _loseRadius;
                var losePoint2 = startPoint + forward * x * _loseRadius + right * y * -_loseRadius;

                Debug.DrawLine(startPoint, nearClipPoint1, _colorNearClip);
                Debug.DrawLine(startPoint, nearClipPoint2, _colorNearClip);
                Debug.DrawLine(nearClipPoint1, viewPoint1, _colorView);
                Debug.DrawLine(nearClipPoint2, viewPoint2, _colorView);
                Debug.DrawLine(viewPoint1, losePoint1, _colorLose);
                Debug.DrawLine(viewPoint2, losePoint2, _colorLose);
            }

            ExtendedDebug.DrawCurve(startPoint, forward, right, angleRadians, _nearClipRadius, _colorView, _curveStep);
            ExtendedDebug.DrawCurve(startPoint, forward, right, angleRadians, _viewRadius, _colorView, _curveStep);
            ExtendedDebug.DrawCurve(startPoint, forward, right, angleRadians, _loseRadius, _colorLose, _curveStep);
        }
#endif
    }
}