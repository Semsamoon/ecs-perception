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

#if UNITY_EDITOR
            foreach (var (_, entity) in
                     SystemAPI
                         .Query<RefRO<SightSenseQueryComponent>>()
                         .WithAll<SightSenseRememberTag>()
                         .WithDisabled<SightSenseVisibilityTag>()
                         .WithEntityAccess())
            {
                UnityEngine.Debug.LogError(
                    $"Entity {entity} has enabled SightSenseRememberTag but disabled SightSenseVisibilityTag - that is not correct! Disable SightSenseRememberTag or enable SightSenseVisibilityTag.");
                buffer.SetComponentEnabled<SightSenseRememberTag>(entity, false);
            }
#endif

            foreach (var (query, entity) in
                     SystemAPI
                         .Query<RefRW<SightSenseQueryComponent>>()
                         .WithDisabled<SightSenseVisibilityTag, SightSenseRememberTag>()
                         .WithEntityAccess())
            {
                var (listenerEntity, sourceEntity) = query.ValueRO;
                var listenerLocalToWorld = SystemAPI.GetComponentRO<LocalToWorld>(listenerEntity);
                var listener = SystemAPI.GetComponentRO<SightSenseListenerComponent>(listenerEntity);
                var sourcePosition = SystemAPI.GetComponentRO<LocalToWorld>(sourceEntity).ValueRO.Position;

                if (!IsInSightCone(listenerLocalToWorld.ValueRO, listener.ValueRO, sourcePosition, false)
                    || !IsRayConnectsDirectly(listenerEntity, listenerLocalToWorld.ValueRO, listener.ValueRO, sourceEntity, sourcePosition,
                        collisionWorld))
                    continue;

                query.ValueRW.SourcePosition = sourcePosition;
                buffer.SetComponentEnabled<SightSenseVisibilityTag>(entity, true);
            }

            foreach (var (query, entity) in
                     SystemAPI
                         .Query<RefRW<SightSenseQueryComponent>>()
                         .WithAll<SightSenseVisibilityTag>()
                         .WithDisabled<SightSenseRememberTag>()
                         .WithEntityAccess())
            {
                var (listenerEntity, sourceEntity) = query.ValueRO;
                var listenerLocalToWorld = SystemAPI.GetComponentRO<LocalToWorld>(listenerEntity);
                var listener = SystemAPI.GetComponentRO<SightSenseListenerComponent>(listenerEntity);
                var sourcePosition = SystemAPI.GetComponentRO<LocalToWorld>(sourceEntity).ValueRO.Position;

                query.ValueRW.SourcePosition = sourcePosition;

                if (IsInSightCone(listenerLocalToWorld.ValueRO, listener.ValueRO, sourcePosition, true)
                    && IsRayConnectsDirectly(listenerEntity, listenerLocalToWorld.ValueRO, listener.ValueRO, sourceEntity, sourcePosition,
                        collisionWorld))
                    continue;

                query.ValueRW.RememberTime = listener.ValueRO.RememberTime;
                buffer.SetComponentEnabled<SightSenseRememberTag>(entity, true);
            }

            foreach (var (query, entity) in
                     SystemAPI
                         .Query<RefRW<SightSenseQueryComponent>>()
                         .WithAll<SightSenseVisibilityTag, SightSenseRememberTag>()
                         .WithEntityAccess())
            {
                var (listenerEntity, sourceEntity, sourcePosition) = query.ValueRO;
                var observerLocalToWorld = SystemAPI.GetComponentRO<LocalToWorld>(listenerEntity);
                var listener = SystemAPI.GetComponentRO<SightSenseListenerComponent>(listenerEntity);

                query.ValueRW.RememberTime -= SystemAPI.Time.DeltaTime;

                if (IsInSightCone(observerLocalToWorld.ValueRO, listener.ValueRO, sourcePosition, true)
                    && IsRayConnectsDirectly(listenerEntity, observerLocalToWorld.ValueRO, listener.ValueRO, sourceEntity, sourcePosition,
                        collisionWorld))
                {
                    buffer.SetComponentEnabled<SightSenseRememberTag>(entity, false);
                    continue;
                }

                if (query.ValueRW.RememberTime > 0) continue;

                buffer.SetComponentEnabled<SightSenseVisibilityTag>(entity, false);
                buffer.SetComponentEnabled<SightSenseRememberTag>(entity, false);
            }

            buffer.Playback(state.EntityManager);
        }

        [BurstCompile]
        private bool IsInSightCone(
            in LocalToWorld listenerLocalToWorld, in SightSenseListenerComponent listener, in float3 sourcePosition, bool isExtendToLoseRadius)
        {
            var position = listenerLocalToWorld.Position + listenerLocalToWorld.Forward * -listener.BackwardOffset;
            var difference = sourcePosition - position;
            var distanceSquared = math.lengthsq(difference);

            if (distanceSquared > (!isExtendToLoseRadius ? listener.ViewRadiusSquared : listener.LoseRadiusSquared)
                || distanceSquared < listener.NearClipRadiusSquared) return false;

            var direction = difference * math.rsqrt(distanceSquared);
            return math.dot(listenerLocalToWorld.Forward, direction) >= listener.ViewAngleCos;
        }

        [BurstCompile]
        private bool IsRayConnectsDirectly(
            Entity listenerEntity, in LocalToWorld listenerLocalToWorld, in SightSenseListenerComponent listener,
            Entity sourceEntity, in float3 sourcePosition, CollisionWorld collisionWorld)
        {
            var raycast = new RaycastInput
            {
                Start = listenerLocalToWorld.Position + listenerLocalToWorld.Forward * -listener.BackwardOffset,
                End = sourcePosition,
                Filter = CollisionFilter.Default,
            };

            var hits = new NativeList<RaycastHit>(4, Allocator.Temp);
            collisionWorld.CastRay(raycast, ref hits);

            foreach (var hit in hits)
            {
                if (hit.Entity != listenerEntity && hit.Entity != sourceEntity) return false;
            }

            return true;
        }
    }
}