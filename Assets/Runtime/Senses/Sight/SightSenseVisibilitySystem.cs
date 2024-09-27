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
                         .Query<RefRO<ComponentSenseInteractionRemember>>()
                         .WithAll<SightSenseRememberTag>()
                         .WithDisabled<TagSenseFeel>()
                         .WithEntityAccess())
            {
                UnityEngine.Debug.LogError(
                    $"Entity {entity} has enabled SightSenseRememberTag but disabled SightSenseVisibilityTag - that is not correct! Disable SightSenseRememberTag or enable SightSenseVisibilityTag.");
                buffer.SetComponentEnabled<SightSenseRememberTag>(entity, false);
            }
#endif

            foreach (var (interaction, remember, entityInteraction) in
                     SystemAPI
                         .Query<RefRW<ComponentSenseInteraction>, RefRW<ComponentSenseInteractionRemember>>()
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

                remember.ValueRW.SourcePosition = sourcePosition;
                buffer.SetComponentEnabled<TagSenseFeel>(entityInteraction, true);
            }

            foreach (var (interaction, remember, entity) in
                     SystemAPI
                         .Query<RefRW<ComponentSenseInteraction>, RefRW<ComponentSenseInteractionRemember>>()
                         .WithAll<TagSenseFeel>()
                         .WithDisabled<SightSenseRememberTag>()
                         .WithEntityAccess())
            {
                var (entityReceiver, entitySource) = interaction.ValueRO;
                var receiverLocalToWorld = SystemAPI.GetComponentRO<LocalToWorld>(entityReceiver);
                var listener = SystemAPI.GetComponentRO<SightSenseListenerComponent>(entityReceiver);
                var sourcePosition = SystemAPI.GetComponentRO<LocalToWorld>(entitySource).ValueRO.Position;

                remember.ValueRW.SourcePosition = sourcePosition;

                if (IsInSightCone(receiverLocalToWorld.ValueRO, listener.ValueRO, sourcePosition, true)
                    && IsRayConnectsDirectly(entityReceiver, receiverLocalToWorld.ValueRO, listener.ValueRO, entitySource, sourcePosition,
                        collisionWorld))
                    continue;

                remember.ValueRW.Duration = listener.ValueRO.RememberTime;
                buffer.SetComponentEnabled<SightSenseRememberTag>(entity, true);
            }

            foreach (var (interaction, remember, entity) in
                     SystemAPI
                         .Query<RefRW<ComponentSenseInteraction>, RefRW<ComponentSenseInteractionRemember>>()
                         .WithAll<TagSenseFeel, SightSenseRememberTag>()
                         .WithEntityAccess())
            {
                remember.ValueRW.Duration -= SystemAPI.Time.DeltaTime;

                var (entityReceiver, entitySource) = interaction.ValueRO;
                var (sourcePosition, rememberTime) = remember.ValueRO;
                var receiverLocalToWorld = SystemAPI.GetComponentRO<LocalToWorld>(entityReceiver);
                var listener = SystemAPI.GetComponentRO<SightSenseListenerComponent>(entityReceiver);

                if (IsInSightCone(receiverLocalToWorld.ValueRO, listener.ValueRO, sourcePosition, true)
                    && IsRayConnectsDirectly(entityReceiver, receiverLocalToWorld.ValueRO, listener.ValueRO, entitySource, sourcePosition,
                        collisionWorld))
                {
                    buffer.SetComponentEnabled<SightSenseRememberTag>(entity, false);
                    continue;
                }

                if (rememberTime > 0) continue;

                buffer.SetComponentEnabled<TagSenseFeel>(entity, false);
                buffer.SetComponentEnabled<SightSenseRememberTag>(entity, false);
            }

            buffer.Playback(state.EntityManager);
        }

        [BurstCompile]
        private bool IsInSightCone(
            in LocalToWorld receiverLocalToWorld, in SightSenseListenerComponent listener, in float3 sourcePosition, bool isExtendToLoseRadius)
        {
            var position = receiverLocalToWorld.Position + receiverLocalToWorld.Forward * -listener.BackwardOffset;
            var difference = sourcePosition - position;
            var distanceSquared = math.lengthsq(difference);

            if (distanceSquared > (!isExtendToLoseRadius ? listener.ViewRadiusSquared : listener.LoseRadiusSquared)
                || distanceSquared < listener.NearClipRadiusSquared) return false;

            var direction = difference * math.rsqrt(distanceSquared);
            return math.dot(receiverLocalToWorld.Forward, direction) >= listener.ViewAngleCos;
        }

        [BurstCompile]
        private bool IsRayConnectsDirectly(
            Entity entityReceiver, in LocalToWorld receiverLocalToWorld, in SightSenseListenerComponent listener,
            Entity entitySource, in float3 sourcePosition, CollisionWorld collisionWorld)
        {
            var raycast = new RaycastInput
            {
                Start = receiverLocalToWorld.Position + receiverLocalToWorld.Forward * -listener.BackwardOffset,
                End = sourcePosition,
                Filter = CollisionFilter.Default,
            };

            var hits = new NativeList<RaycastHit>(4, Allocator.Temp);
            collisionWorld.CastRay(raycast, ref hits);

            foreach (var hit in hits)
            {
                if (hit.Entity != entityReceiver && hit.Entity != entitySource) return false;
            }

            return true;
        }
    }
}