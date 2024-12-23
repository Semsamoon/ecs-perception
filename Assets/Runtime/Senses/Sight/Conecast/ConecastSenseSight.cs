using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECSPerception.Sight
{
    [BurstCompile]
    public struct ConecastSenseSight
    {
        public static bool IsInside(RefRO<ComponentSenseSightReceiver> receiverData, RefRO<LocalToWorld> receiverTransform,
            RefRO<ComponentSenseSightSource> sourceData, RefRO<LocalToWorld> sourceTransform,
            bool isExtendToLoseRadius)
        {
            return new ConecastSenseSight().IsInsideInternal(receiverData, receiverTransform, sourceData, sourceTransform, isExtendToLoseRadius);
        }

        [BurstCompile]
        private bool IsInsideInternal(RefRO<ComponentSenseSightReceiver> receiverData, RefRO<LocalToWorld> receiverTransform,
            RefRO<ComponentSenseSightSource> sourceData, RefRO<LocalToWorld> sourceTransform,
            bool isExtendToLoseRadius)
        {
            var receiverPosition = receiverTransform.ValueRO.Value.TransformPoint(receiverData.ValueRO.Offset);
            var difference = sourceTransform.ValueRO.Value.TransformPoint(sourceData.ValueRO.Offset) - receiverPosition;
            var distanceSquared = math.lengthsq(difference);

            if (distanceSquared > (!isExtendToLoseRadius ? receiverData.ValueRO.ViewRadiusSquared : receiverData.ValueRO.LoseRadiusSquared)
                || distanceSquared < receiverData.ValueRO.NearClipRadiusSquared)
            {
                return false;
            }

            var direction = difference * math.rsqrt(distanceSquared);
            return math.dot(receiverTransform.ValueRO.Forward, direction) >= receiverData.ValueRO.ViewAngleCos;
        }
    }
}