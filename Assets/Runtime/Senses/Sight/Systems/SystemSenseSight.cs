using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace PerceptionECS
{
    [BurstCompile]
    public partial struct SystemSenseSight : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var buffer = new EntityCommandBuffer(Allocator.Temp);
            var collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;

            foreach (var (interaction, remember, entityInteraction) in
                     SystemAPI
                         .Query<RefRO<ComponentSenseInteraction>, RefRW<ComponentSenseInteractionRemember>>()
                         .WithAll<TagSenseNeglect, TagSenseSightInteraction>()
                         .WithEntityAccess())
            {
                var (entityReceiver, entitySource) = interaction.ValueRO;
                var receiver = SystemAPI.GetComponentRO<ComponentSenseSightReceiver>(entityReceiver).ValueRO;
                var receiverTransform = SystemAPI.GetComponentRO<LocalToWorld>(entityReceiver).ValueRO;
                var sourcePosition = SystemAPI.GetComponentRO<LocalToWorld>(entitySource).ValueRO.Position;

                if (!IsInSightCone(receiverTransform, receiver, sourcePosition, false)
                    || !IsRayConnectsDirectly(entityReceiver, receiverTransform, receiver, entitySource, sourcePosition, collisionWorld))
                {
                    continue;
                }

                remember.ValueRW.SourcePosition = sourcePosition;
                buffer.SetComponentEnabled<TagSenseNeglect>(entityInteraction, false);
                buffer.SetComponentEnabled<TagSenseFeel>(entityInteraction, true);
            }

            foreach (var (interaction, remember, entityInteraction) in
                     SystemAPI
                         .Query<RefRO<ComponentSenseInteraction>, RefRW<ComponentSenseInteractionRemember>>()
                         .WithAll<TagSenseFeel, TagSenseSightInteraction>()
                         .WithEntityAccess())
            {
                var (entityReceiver, entitySource) = interaction.ValueRO;
                var receiver = SystemAPI.GetComponentRO<ComponentSenseSightReceiver>(entityReceiver).ValueRO;
                var receiverTransform = SystemAPI.GetComponentRO<LocalToWorld>(entityReceiver).ValueRO;
                var sourcePosition = SystemAPI.GetComponentRO<LocalToWorld>(entitySource).ValueRO.Position;


                if (IsInSightCone(receiverTransform, receiver, sourcePosition, true)
                    && IsRayConnectsDirectly(entityReceiver, receiverTransform, receiver, entitySource, sourcePosition, collisionWorld))
                {
                    remember.ValueRW.SourcePosition = sourcePosition;
                    continue;
                }

                remember.ValueRW.Timer = receiver.RememberTime;
                buffer.SetComponentEnabled<TagSenseFeel>(entityInteraction, false);
                buffer.SetComponentEnabled<TagSenseRemember>(entityInteraction, true);
            }

            foreach (var (interaction, remember, entityInteraction) in
                     SystemAPI
                         .Query<RefRO<ComponentSenseInteraction>, RefRW<ComponentSenseInteractionRemember>>()
                         .WithAll<TagSenseRemember, TagSenseSightInteraction>()
                         .WithEntityAccess())
            {
                remember.ValueRW.Timer -= SystemAPI.Time.DeltaTime;

                var (entityReceiver, entitySource) = interaction.ValueRO;
                var receiver = SystemAPI.GetComponentRO<ComponentSenseSightReceiver>(entityReceiver).ValueRO;
                var receiverTransform = SystemAPI.GetComponentRO<LocalToWorld>(entityReceiver).ValueRO;
                var sourcePosition = SystemAPI.GetComponentRO<LocalToWorld>(entitySource).ValueRO.Position;
                var (_, timer) = remember.ValueRO;

                if (IsInSightCone(receiverTransform, receiver, sourcePosition, true)
                    && IsRayConnectsDirectly(entityReceiver, receiverTransform, receiver, entitySource, sourcePosition, collisionWorld))
                {
                    remember.ValueRW.SourcePosition = sourcePosition;
                    buffer.SetComponentEnabled<TagSenseRemember>(entityInteraction, false);
                    buffer.SetComponentEnabled<TagSenseFeel>(entityInteraction, true);
                    continue;
                }

                if (timer > 0)
                {
                    continue;
                }

                buffer.SetComponentEnabled<TagSenseRemember>(entityInteraction, false);
                buffer.SetComponentEnabled<TagSenseNeglect>(entityInteraction, true);
            }

            buffer.Playback(state.EntityManager);
        }

        [BurstCompile]
        private bool IsInSightCone(
            in LocalToWorld receiverTransform, in ComponentSenseSightReceiver receiver, in float3 sourcePosition, bool isExtendToLoseRadius)
        {
            var position = receiverTransform.Position + receiverTransform.Forward * -receiver.BackwardOffset;
            var difference = sourcePosition - position;
            var distanceSquared = math.lengthsq(difference);

            if (distanceSquared > (!isExtendToLoseRadius ? receiver.ViewRadiusSquared : receiver.LoseRadiusSquared)
                || distanceSquared < receiver.NearClipRadiusSquared)
            {
                return false;
            }

            var direction = difference * math.rsqrt(distanceSquared);
            return math.dot(receiverTransform.Forward, direction) >= receiver.ViewAngleCos;
        }

        [BurstCompile]
        private bool IsRayConnectsDirectly(
            Entity entityReceiver, in LocalToWorld receiverTransform, in ComponentSenseSightReceiver receiver,
            Entity entitySource, in float3 sourcePosition, CollisionWorld collisionWorld)
        {
            var raycast = new RaycastInput
            {
                Start = receiverTransform.Position + receiverTransform.Forward * -receiver.BackwardOffset,
                End = sourcePosition,
                Filter = CollisionFilter.Default,
            };

            var hits = new NativeList<RaycastHit>(4, Allocator.Temp);
            collisionWorld.CastRay(raycast, ref hits);

            foreach (var hit in hits)
            {
                if (hit.Entity != entityReceiver && hit.Entity != entitySource)
                {
                    return false;
                }
            }

            return true;
        }
    }
}