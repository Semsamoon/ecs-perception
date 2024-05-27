using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace PerceptionECS
{
    [BurstCompile]
    public partial struct SightSenseVisibilitySystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var buffer = new EntityCommandBuffer(Allocator.Temp);
            var collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;

            foreach (var (query, entity) in
                     SystemAPI.Query<RefRW<SightSenseQueryComponent>>().WithDisabled<SightSenseVisibilityTag>().WithEntityAccess())
            {
                var (observer, target, _) = query.ValueRO;
                var observerLocalToWorld = SystemAPI.GetComponentRO<LocalToWorld>(observer);
                var listener = SystemAPI.GetComponentRO<SightSenseListenerComponent>(observer);
                var targetPosition = SystemAPI.GetComponentRO<LocalToWorld>(target).ValueRO.Position;

                if (!IsInSightCone(observerLocalToWorld.ValueRO, listener.ValueRO, targetPosition, false)
                    || !IsRayConnectsDirectly(observer, observerLocalToWorld.ValueRO, listener.ValueRO, target, targetPosition, collisionWorld))
                    continue;

                query.ValueRW.TargetPosition = targetPosition;
                buffer.SetComponentEnabled<SightSenseVisibilityTag>(entity, true);
            }

            foreach (var (query, entity) in
                     SystemAPI.Query<RefRW<SightSenseQueryComponent>>().WithAll<SightSenseVisibilityTag>().WithEntityAccess())
            {
                var (observer, target, _) = query.ValueRO;
                var observerLocalToWorld = SystemAPI.GetComponentRO<LocalToWorld>(observer);
                var listener = SystemAPI.GetComponentRO<SightSenseListenerComponent>(observer);
                var targetPosition = SystemAPI.GetComponentRO<LocalToWorld>(target).ValueRO.Position;

                query.ValueRW.TargetPosition = targetPosition;

                if (IsInSightCone(observerLocalToWorld.ValueRO, listener.ValueRO, targetPosition, true)
                    && IsRayConnectsDirectly(observer, observerLocalToWorld.ValueRO, listener.ValueRO, target, targetPosition, collisionWorld))
                    continue;

                buffer.SetComponentEnabled<SightSenseVisibilityTag>(entity, false);
            }

            buffer.Playback(state.EntityManager);
        }

        [BurstCompile]
        private bool IsInSightCone(
            in LocalToWorld observerLocalToWorld, in SightSenseListenerComponent listener, in float3 targetPosition, bool isExtendToLoseRadius)
        {
            var position = observerLocalToWorld.Position + observerLocalToWorld.Forward * -listener.BackwardOffset;
            var difference = targetPosition - position;
            var distanceSquared = math.lengthsq(difference);

            if (distanceSquared > (!isExtendToLoseRadius ? listener.ViewRadiusSquared : listener.LoseRadiusSquared)
                || distanceSquared < listener.NearClipRadiusSquared) return false;

            var direction = difference * math.rsqrt(distanceSquared);
            return math.dot(observerLocalToWorld.Forward, direction) >= listener.ViewAngleCos;
        }

        [BurstCompile]
        private bool IsRayConnectsDirectly(
            Entity observer, in LocalToWorld observerLocalToWorld, in SightSenseListenerComponent listener,
            Entity target, in float3 targetPosition, CollisionWorld collisionWorld)
        {
            var raycast = new RaycastInput
            {
                Start = observerLocalToWorld.Position + observerLocalToWorld.Forward * -listener.BackwardOffset,
                End = targetPosition,
                Filter = CollisionFilter.Default,
            };

            var hits = new NativeList<RaycastHit>(4, Allocator.Temp);
            collisionWorld.CastRay(raycast, ref hits);

            foreach (var hit in hits)
            {
                if (hit.Entity != observer && hit.Entity != target) return false;
            }

            return true;
        }
    }
}