using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace PerceptionECS
{
    public class SightSenseAuthoring : MonoBehaviour
    {
        [SerializeField] private float _sightRadius = 20;
        [SerializeField] private float _loseSightRadius = 30;
        [SerializeField] private float _peripheralVisionAngleDegrees = 45;
        [SerializeField] private SenseAffiliationFilter _detectionByAffiliation = new();
        [SerializeField] private float _autoSuccessRangeFromLastSeenLocation = 0;
        [SerializeField] private float _pointOfViewBackwardOffset = 2;
        [SerializeField] private float _nearClippingRadius = 1;

        private class Baker : Baker<SightSenseAuthoring>
        {
            public override void Bake(SightSenseAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new SightSenseComponent
                {
                    PeripheralVisionAngleCos = math.cos(math.clamp(math.radians(authoring._peripheralVisionAngleDegrees), 0f, math.PI)),
                    SightRadiusSquared = math.pow(authoring._sightRadius + authoring._pointOfViewBackwardOffset, 2),
                    AutoSuccessRangeSquaredFromLastSeenLocation = authoring._autoSuccessRangeFromLastSeenLocation == -1
                        ? -1
                        : math.pow(authoring._autoSuccessRangeFromLastSeenLocation, 2),
                    LoseSightRadiusSquared = math.pow(authoring._loseSightRadius + authoring._pointOfViewBackwardOffset, 2),
                    PointOfViewBackwardOffset = authoring._pointOfViewBackwardOffset,
                    NearClippingRadiusSquared = math.pow(authoring._nearClippingRadius, 2),
                    AffiliationFlags = (ushort)authoring._detectionByAffiliation,
                });
            }
        }

        private void OnDrawGizmosSelected()
        {
            var transform = this.transform;
            var forward = transform.forward;
            var right = transform.right;
            var vision = math.tan(math.radians(_peripheralVisionAngleDegrees / 2));
            var nearClip = vision * _nearClippingRadius;
            var farClip = vision * _sightRadius;
            var loseClip = vision * _loseSightRadius;

            var startPoint = transform.position + forward * -_pointOfViewBackwardOffset;
            var nearClipPoint1 = startPoint + forward * _nearClippingRadius + right * nearClip;
            var nearClipPoint2 = startPoint + forward * _nearClippingRadius + right * -nearClip;
            var farClipPoint1 = startPoint + forward * _sightRadius + right * farClip;
            var farClipPoint2 = startPoint + forward * _sightRadius + right * -farClip;
            var loseClipPoint1 = startPoint + forward * _loseSightRadius + right * loseClip;
            var loseClipPoint2 = startPoint + forward * _loseSightRadius + right * -loseClip;

            Gizmos.color = Color.gray;
            Gizmos.DrawLine(startPoint, nearClipPoint1);
            Gizmos.DrawLine(startPoint, nearClipPoint2);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(nearClipPoint1, nearClipPoint2);
            Gizmos.DrawLine(nearClipPoint1, farClipPoint1);
            Gizmos.DrawLine(nearClipPoint2, farClipPoint2);
            Gizmos.DrawLine(farClipPoint1, farClipPoint2);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(farClipPoint1, loseClipPoint1);
            Gizmos.DrawLine(farClipPoint2, loseClipPoint2);
            Gizmos.DrawLine(loseClipPoint1, loseClipPoint2);
        }
    }

    public struct SightSenseComponent : IComponentData
    {
        public float PeripheralVisionAngleCos;
        public float SightRadiusSquared;
        public float AutoSuccessRangeSquaredFromLastSeenLocation;
        public float LoseSightRadiusSquared;
        public float PointOfViewBackwardOffset;
        public float NearClippingRadiusSquared;
        public ushort AffiliationFlags;
    }
}