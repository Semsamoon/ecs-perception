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
                         .WithDisabled<TagSenseFeel>()
                         .WithEntityAccess())
            {
                UnityEngine.Debug.LogError(
                    $"Entity {entity} has enabled SightSenseRememberTag but disabled SightSenseVisibilityTag - that is not correct! Disable SightSenseRememberTag or enable SightSenseVisibilityTag.");
                buffer.SetComponentEnabled<SightSenseRememberTag>(entity, false);
            }
#endif

            foreach (var (interaction, sightQuery, entityInteraction) in
                     SystemAPI
                         .Query<RefRW<ComponentSenseInteraction>, RefRW<SightSenseQueryComponent>>()
                         .WithDisabled<TagSenseFeel, SightSenseRememberTag>()
                         .WithEntityAccess())
            {
                var (entityReceiver, entitySource) = interaction.ValueRO;
                var receiverLocalToWorld = SystemAPI.GetComponentRO<LocalToWorld>(entityReceiver);
                var listener = SystemAPI.GetComponentRO<SightSenseListenerComponent>(entityReceiver);
                var sourcePosition = SystemAPI.GetComponentRO<LocalToWorld>(entitySource).ValueRO.Position;

                if (!IsInSightCone(receiverLocalToWorld.ValueRO, listener.ValueRO, sourcePosition, false)
                    || !IsRayConnectsDirectly(entityReceiver, receiverLocalToWorld.ValueRO, listener.ValueRO, entitySource, sourcePosition,
                        collisionWorld))
                    continue;

                sightQuery.ValueRW.SourcePosition = sourcePosition;
                buffer.SetComponentEnabled<TagSenseFeel>(entityInteraction, true);
            }

            foreach (var (interaction, sightQuery, entity) in
                     SystemAPI
                         .Query<RefRW<ComponentSenseInteraction>, RefRW<SightSenseQueryComponent>>()
                         .WithAll<TagSenseFeel>()
                         .WithDisabled<SightSenseRememberTag>()
                         .WithEntityAccess())
            {
                var (entityReceiver, entitySource) = interaction.ValueRO;
                var receiverLocalToWorld = SystemAPI.GetComponentRO<LocalToWorld>(entityReceiver);
                var listener = SystemAPI.GetComponentRO<SightSenseListenerComponent>(entityReceiver);
                var sourcePosition = SystemAPI.GetComponentRO<LocalToWorld>(entitySource).ValueRO.Position;

                sightQuery.ValueRW.SourcePosition = sourcePosition;

                if (IsInSightCone(receiverLocalToWorld.ValueRO, listener.ValueRO, sourcePosition, true)
                    && IsRayConnectsDirectly(entityReceiver, receiverLocalToWorld.ValueRO, listener.ValueRO, entitySource, sourcePosition,
                        collisionWorld))
                    continue;

                sightQuery.ValueRW.RememberTime = listener.ValueRO.RememberTime;
                buffer.SetComponentEnabled<SightSenseRememberTag>(entity, true);
            }

            foreach (var (interaction, sightQuery, entity) in
                     SystemAPI
                         .Query<RefRW<ComponentSenseInteraction>, RefRW<SightSenseQueryComponent>>()
                         .WithAll<TagSenseFeel, SightSenseRememberTag>()
                         .WithEntityAccess())
            {
                var (entityReceiver, entitySource) = interaction.ValueRO;
                var (sourcePosition, _) = sightQuery.ValueRO;
                var receiverLocalToWorld = SystemAPI.GetComponentRO<LocalToWorld>(entityReceiver);
                var listener = SystemAPI.GetComponentRO<SightSenseListenerComponent>(entityReceiver);

                sightQuery.ValueRW.RememberTime -= SystemAPI.Time.DeltaTime;

                if (IsInSightCone(receiverLocalToWorld.ValueRO, listener.ValueRO, sourcePosition, true)
                    && IsRayConnectsDirectly(entityReceiver, receiverLocalToWorld.ValueRO, listener.ValueRO, entitySource, sourcePosition,
                        collisionWorld))
                {
                    buffer.SetComponentEnabled<SightSenseRememberTag>(entity, false);
                    continue;
                }

                if (sightQuery.ValueRW.RememberTime > 0) continue;

                buffer.SetComponentEnabled<TagSenseFeel>(entity, false);
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