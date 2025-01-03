using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;

namespace ECSPerception.Sight
{
    [BurstCompile]
    public struct RaycastSenseSightJobCast : IJobParallelFor
    {
        [ReadOnly] public CollisionWorld CollisionWorld;
        [ReadOnly] public NativeArray<RaycastSenseSightCast> Raycasts;
        [WriteOnly] public NativeArray<bool> Results;

        [BurstCompile]
        public void Execute(int index)
        {
            var collisionWorld = CollisionWorld;

            var raycast = new RaycastInput
            {
                Start = Raycasts[index].ReceiverPosition,
                End = Raycasts[index].SourcePosition,
                Filter = CollisionFilter.Default,
            };

            var distanceSquared = math.distancesq(Raycasts[index].ReceiverPosition, Raycasts[index].SourcePosition);
            var minFraction = Raycasts[index].NearClipRadiusSquared / distanceSquared;

            var hitCollector = new RaycastSenseSightCollector<RaycastHit>(Raycasts[index].Receiver, minFraction);
            collisionWorld.CastRay(raycast, ref hitCollector);

            Results[index] = hitCollector.ClosestHit.Entity == Raycasts[index].Source;
        }
    }
}