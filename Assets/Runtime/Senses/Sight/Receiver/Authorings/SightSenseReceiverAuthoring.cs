using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ECSPerception
{
    public sealed class SightSenseReceiverAuthoring : MonoBehaviour
    {
        [SerializeField] private float _viewRadius = 5;
        [SerializeField] private float _loseRadius = 7;
        [SerializeField, Min(0)] private float _nearClipRadius = 0.5f;
        [SerializeField] private float _backwardOffset = 2;
        [SerializeField, Range(0, 360)] private float _viewAngleDegrees = 90;
        [SerializeField, Min(0)] private float _rememberTime = 1;

        private sealed class Baker : Baker<SightSenseReceiverAuthoring>
        {
            public override void Bake(SightSenseReceiverAuthoring authoring)
            {
                var entity = CreateAdditionalEntity(TransformUsageFlags.None);
                AddComponent(entity, new EventSenseReceiverCreate
                {
                    Owner = GetEntity(TransformUsageFlags.Dynamic),
                    Transform = GetEntity(TransformUsageFlags.Dynamic),
                    RememberTime = authoring._rememberTime,
                });
                AddComponent(entity, new EventSenseSightReceiverCreate
                {
                    ViewAngleCos = math.cos(math.radians(authoring._viewAngleDegrees / 2)),
                    ViewRadiusSquared = math.pow(authoring._viewRadius, 2),
                    LoseRadiusSquared = math.pow(authoring._loseRadius, 2),
                    BackwardOffset = authoring._backwardOffset,
                    NearClipRadiusSquared = math.pow(authoring._nearClipRadius, 2),
                });
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            _viewRadius = math.max(_viewRadius, _nearClipRadius);
            _loseRadius = math.max(_loseRadius, _viewRadius);
        }

        private void OnDrawGizmos()
        {
            var transform = this.transform;
            var forward = transform.forward;
            var right = transform.right;

            var vision_x = math.cos(math.radians(_viewAngleDegrees / 2));
            var vision_y = math.sin(math.radians(_viewAngleDegrees / 2));

            var startPoint = transform.position + forward * -_backwardOffset;

            if (_viewAngleDegrees < 360)
            {
                var nearClipPoint1 = startPoint + forward * vision_x * _nearClipRadius + right * vision_y * _nearClipRadius;
                var nearClipPoint2 = startPoint + forward * vision_x * _nearClipRadius + right * -vision_y * _nearClipRadius;
                var farClipPoint1 = startPoint + forward * vision_x * _viewRadius + right * vision_y * _viewRadius;
                var farClipPoint2 = startPoint + forward * vision_x * _viewRadius + right * vision_y * -_viewRadius;
                var loseClipPoint1 = startPoint + forward * vision_x * _loseRadius + right * vision_y * _loseRadius;
                var loseClipPoint2 = startPoint + forward * vision_x * _loseRadius + right * vision_y * -_loseRadius;

                Gizmos.color = Color.gray;
                Gizmos.DrawLine(startPoint, nearClipPoint1);
                Gizmos.DrawLine(startPoint, nearClipPoint2);

                Gizmos.color = Color.green;
                Gizmos.DrawLine(nearClipPoint1, farClipPoint1);
                Gizmos.DrawLine(nearClipPoint2, farClipPoint2);

                Gizmos.color = Color.red;
                Gizmos.DrawLine(farClipPoint1, loseClipPoint1);
                Gizmos.DrawLine(farClipPoint2, loseClipPoint2);
            }

            if (_viewAngleDegrees == 0) return;

            for (var i = 0; i < _viewAngleDegrees / 2; i += 2)
            {
                var previous_x = math.cos(math.radians(i));
                var previous_y = math.sin(math.radians(i));
                var current_x = math.cos(math.radians(math.clamp(i + 2, 0, _viewAngleDegrees / 2)));
                var current_y = math.sin(math.radians(math.clamp(i + 2, 0, _viewAngleDegrees / 2)));

                Gizmos.color = Color.green;
                Gizmos.DrawLine(
                    startPoint + forward * previous_x * _nearClipRadius + right * previous_y * _nearClipRadius,
                    startPoint + forward * current_x * _nearClipRadius + right * current_y * _nearClipRadius);
                Gizmos.DrawLine(
                    startPoint + forward * previous_x * _nearClipRadius + right * -previous_y * _nearClipRadius,
                    startPoint + forward * current_x * _nearClipRadius + right * -current_y * _nearClipRadius);

                Gizmos.color = Color.red;
                Gizmos.DrawLine(
                    startPoint + forward * previous_x * _loseRadius + right * previous_y * _loseRadius,
                    startPoint + forward * current_x * _loseRadius + right * current_y * _loseRadius);
                Gizmos.DrawLine(
                    startPoint + forward * previous_x * _loseRadius + right * -previous_y * _loseRadius,
                    startPoint + forward * current_x * _loseRadius + right * -current_y * _loseRadius);

                Gizmos.color = Color.green;
                Gizmos.DrawLine(
                    startPoint + forward * previous_x * _viewRadius + right * previous_y * _viewRadius,
                    startPoint + forward * current_x * _viewRadius + right * current_y * _viewRadius);
                Gizmos.DrawLine(
                    startPoint + forward * previous_x * _viewRadius + right * -previous_y * _viewRadius,
                    startPoint + forward * current_x * _viewRadius + right * -current_y * _viewRadius);
            }
        }
#endif
    }
}