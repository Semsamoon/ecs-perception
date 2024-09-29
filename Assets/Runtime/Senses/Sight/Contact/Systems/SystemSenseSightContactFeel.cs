using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace PerceptionECS
{
    [BurstCompile, UpdateInGroup(typeof(GroupSenseUpdate), OrderFirst = true)]
    public partial struct SystemSenseSightContactFeel : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var buffer = new EntityCommandBuffer(Allocator.Temp);
            var collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;

            foreach (var (_, entity) in
                     SystemAPI.Query<EventSenseContactFeel>()
                         .WithEntityAccess())
            {
                buffer.SetComponentEnabled<EventSenseContactFeel>(entity, false);
            }

            foreach (var (contact, entity) in
                     SystemAPI.Query<ComponentSenseContact>()
                         .WithAll<TagSenseSight>()
                         .WithDisabled<TagSenseContactFeel>()
                         .WithEntityAccess())
            {
                var entityReceiver = SystemAPI.GetComponentRO<ComponentSenseBase>(contact.Receiver).ValueRO.Entity;
                var entitySource = SystemAPI.GetComponentRO<ComponentSenseBase>(contact.Source).ValueRO.Entity;

                var receiver = SystemAPI.GetComponentRO<ComponentSenseSightReceiver>(contact.Receiver).ValueRO;
                var receiverTransform = SystemAPI.GetComponentRO<LocalToWorld>(entityReceiver).ValueRO;
                var sourcePosition = SystemAPI.GetComponentRO<LocalToWorld>(entitySource).ValueRO.Position;

                if (!IsInSightCone(receiverTransform, receiver, sourcePosition, false)
                    || !IsRayConnectsDirectly(entityReceiver, receiverTransform, receiver, entitySource, sourcePosition, ref collisionWorld))
                {
                    continue;
                }

                buffer.SetComponentEnabled<TagSenseContactFeel>(entity, true);
                buffer.SetComponentEnabled<EventSenseContactFeel>(entity, true);
            }

            foreach (var (contact, entity) in
                     SystemAPI.Query<ComponentSenseContact>()
                         .WithAll<TagSenseSight, TagSenseContactFeel>()
                         .WithEntityAccess())
            {
                var entityReceiver = SystemAPI.GetComponentRO<ComponentSenseBase>(contact.Receiver).ValueRO.Entity;
                var entitySource = SystemAPI.GetComponentRO<ComponentSenseBase>(contact.Source).ValueRO.Entity;

                var receiver = SystemAPI.GetComponentRO<ComponentSenseSightReceiver>(contact.Receiver).ValueRO;
                var receiverTransform = SystemAPI.GetComponentRO<LocalToWorld>(entityReceiver).ValueRO;
                var sourceTransform = SystemAPI.GetComponentRO<LocalToWorld>(entitySource).ValueRO;

                if (IsInSightCone(receiverTransform, receiver, sourceTransform.Position, true)
                    && IsRayConnectsDirectly(entityReceiver, receiverTransform, receiver, entitySource, sourceTransform.Position, ref collisionWorld))
                {
                    continue;
                }

                buffer.SetComponentEnabled<TagSenseContactFeel>(entity, false);
                buffer.SetComponentEnabled<EventSenseContactFeel>(entity, true);
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