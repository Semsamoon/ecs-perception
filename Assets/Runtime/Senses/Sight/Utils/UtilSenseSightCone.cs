using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECSPerception.Sight
{
    [BurstCompile]
    public struct UtilSenseSightCone
    {
        public static bool Cast(RefRO<ComponentSenseSightReceiver> receiverData, RefRO<LocalToWorld> receiverTransform,
            RefRO<ComponentSenseSightSource> sourceData, RefRO<LocalToWorld> sourceTransform, bool isExtendedRadius)
        {
            return new UtilSenseSightCone().CastInternal(receiverData, receiverTransform, sourceData, sourceTransform, isExtendedRadius);
        }

        [BurstCompile]
        private bool CastInternal(RefRO<ComponentSenseSightReceiver> receiverData, RefRO<LocalToWorld> receiverTransform,
            RefRO<ComponentSenseSightSource> sourceData, RefRO<LocalToWorld> sourceTransform, bool isExtendedRadius)
        {
            var receiverPosition = receiverTransform.ValueRO.Value.TransformPoint(receiverData.ValueRO.Offset);
            var difference = sourceTransform.ValueRO.Value.TransformPoint(sourceData.ValueRO.Offset) - receiverPosition;
            var distanceSquared = math.lengthsq(difference);

            if (distanceSquared > (!isExtendedRadius ? receiverData.ValueRO.ViewRadiusSquared : receiverData.ValueRO.LoseRadiusSquared)
                || distanceSquared < receiverData.ValueRO.NearClipRadiusSquared)
            {
                return false;
            }

            var direction = difference * math.rsqrt(distanceSquared);
            return math.dot(receiverTransform.ValueRO.Forward, direction) >= receiverData.ValueRO.ViewAngleCos;
        }
    }
}