using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;

namespace ECSPerception.Sight
{
    [BurstCompile]
    public struct UtilSenseSightRayJob : IJobParallelFor
    {
        [ReadOnly] public CollisionWorld CollisionWorld;
        [ReadOnly] public NativeArray<BufferSenseSightExecute> Executes;
        [WriteOnly] public NativeArray<bool> Results;

        [BurstCompile]
        public void Execute(int index)
        {
            var collisionWorld = CollisionWorld;
            var execute = Executes[index];

            var raycast = new RaycastInput
            {
                Start = execute.ReceiverPosition,
                End = execute.SourcePosition,
                Filter = CollisionFilter.Default,
            };

            var distanceSquared = math.distancesq(execute.ReceiverPosition, execute.SourcePosition);
            var minFraction = execute.NearClipRadiusSquared / distanceSquared;

            var hitCollector = new UtilSenseSightRayCollector<RaycastHit>(execute.Receiver, minFraction);
            collisionWorld.CastRay(raycast, ref hitCollector);

            Results[index] = hitCollector.ClosestHit.Entity == execute.Source;
        }
    }
}