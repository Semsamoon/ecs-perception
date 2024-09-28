using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace PerceptionECS
{
    [BurstCompile, UpdateAfter(typeof(SystemSenseTransition)), UpdateBefore(typeof(SystemSenseRemember))]
    public partial struct SystemSenseSight : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var buffer = new EntityCommandBuffer(Allocator.Temp);
            var collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;

            foreach (var (interaction, entityInteraction) in
                     SystemAPI.Query<ComponentSenseInteraction>().WithAll<TagSenseNeglect, TagSenseSightInteraction>().WithEntityAccess())
            {
                var (entityReceiver, entitySource) = interaction;
                var receiver = SystemAPI.GetComponentRO<ComponentSenseSightReceiver>(entityReceiver).ValueRO;
                var receiverTransform = SystemAPI.GetComponentRO<LocalToWorld>(entityReceiver).ValueRO;
                var sourcePosition = SystemAPI.GetComponentRO<LocalToWorld>(entitySource).ValueRO.Position;

                if (!IsInSightCone(receiverTransform, receiver, sourcePosition, false)
                    || !IsRayConnectsDirectly(entityReceiver, receiverTransform, receiver, entitySource, sourcePosition, ref collisionWorld))
                {
                    continue;
                }

                buffer.SetComponentEnabled<TagSenseNeglect>(entityInteraction, false);
                buffer.SetComponentEnabled<TagSenseFeel>(entityInteraction, true);
                buffer.SetComponentEnabled<TagSenseTransitionNeglectToFeel>(entityInteraction, true);
            }

            foreach (var (interaction, entityInteraction) in
                     SystemAPI.Query<ComponentSenseInteraction>().WithAll<TagSenseFeel, TagSenseSightInteraction>().WithEntityAccess())
            {
                var (entityReceiver, entitySource) = interaction;
                var receiver = SystemAPI.GetComponentRO<ComponentSenseSightReceiver>(entityReceiver).ValueRO;
                var receiverTransform = SystemAPI.GetComponentRO<LocalToWorld>(entityReceiver).ValueRO;
                var sourceTransform = SystemAPI.GetComponentRO<LocalToWorld>(entitySource).ValueRO;

                if (IsInSightCone(receiverTransform, receiver, sourceTransform.Position, true)
                    && IsRayConnectsDirectly(entityReceiver, receiverTransform, receiver, entitySource, sourceTransform.Position, ref collisionWorld))
                {
                    continue;
                }

                buffer.SetComponentEnabled<TagSenseFeel>(entityInteraction, false);
                buffer.SetComponentEnabled<TagSenseNeglect>(entityInteraction, true);
                buffer.SetComponentEnabled<TagSenseTransitionFeelToNeglect>(entityInteraction, true);
            }

            buffer.Playback(state.EntityManager);
        }

        [BurstCompile]
        private bool IsInSightCone(
            in LocalToWorld receiverTransform, in ComponentSenseSightReceiver receiver,
            in float3 sourcePosition, bool isExtendToLoseRadius)
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
            Entity entitySource, in float3 sourcePosition, ref CollisionWorld collisionWorld)
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