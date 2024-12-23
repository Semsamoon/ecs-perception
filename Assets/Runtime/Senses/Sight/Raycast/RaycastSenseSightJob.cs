using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Physics;

namespace ECSPerception.Sight
{
    [BurstCompile]
    public struct RaycastSenseSightJob : IJobParallelFor
    {
        [ReadOnly] public CollisionWorld CollisionWorld;
        [ReadOnly] public NativeArray<RaycastSenseSightData> Raycasts;

        [WriteOnly] public NativeArray<bool> Results;

        [BurstCompile]
        public void Execute(int index)
        {
            var collisionWorldCopy = CollisionWorld;
            var raycast = new RaycastInput
            {
                Start = Raycasts[index].ReceiverPosition,
                End = Raycasts[index].SourcePosition,
                Filter = CollisionFilter.Default,
            };

            var hitCollector = new RaycastSenseSightCollector<RaycastHit>(Raycasts[index].Receiver);
            collisionWorldCopy.CastRay(raycast, ref hitCollector);

            Results[index] = hitCollector.ClosestHit.Entity == Raycasts[index].Source;
        }
    }
}