using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace PerceptionECS
{
    [BurstCompile, UpdateInGroup(typeof(GroupSenseUpdate), OrderFirst = true)]
    public partial struct SystemSenseSightContactUpdateFeel : ISystem
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

            foreach (var (contact, entity) in
                     SystemAPI.Query<RefRO<ComponentSenseContact>>().WithAll<ComponentSenseSightContact>()
                         .WithDisabled<TagSenseContactFeel>().WithEntityAccess())
            {
                var entityReceiver = contact.ValueRO.Receiver;
                var entitySource = contact.ValueRO.Source;

                var receiverComponent = SystemAPI.GetComponentRO<ComponentSenseReceiver>(entityReceiver);
                var sourceComponent = SystemAPI.GetComponentRO<ComponentSenseSource>(entitySource);

                var receiver = SystemAPI.GetComponentRO<ComponentSenseSightReceiver>(entityReceiver).ValueRO;
                var receiverTransform = SystemAPI.GetComponentRO<LocalToWorld>(receiverComponent.ValueRO.Transform).ValueRO;
                var sourceTransform = SystemAPI.GetComponentRO<LocalToWorld>(sourceComponent.ValueRO.Transform).ValueRO;

                if (!IsInSightCone(receiverTransform, receiver, sourceTransform.Position, false)
                    || !IsRayConnectsDirectly(receiverComponent.ValueRO.Owner, receiverTransform, receiver,
                        sourceComponent.ValueRO.Owner, sourceTransform.Position, ref collisionWorld))
                {
                    continue;
                }

                buffer.SetComponentEnabled<TagSenseContactFeel>(entity, true);

                var eventUpdate = buffer.CreateEntity();
                buffer.AddComponent(eventUpdate, new EventSenseContactUpdateFeel
                {
                    Entity = entity,
                });
                buffer.AddComponent(eventUpdate, new EventSenseSightContactUpdateFeel());
            }

            foreach (var (contact, entity) in
                     SystemAPI.Query<RefRO<ComponentSenseContact>>().WithAll<ComponentSenseSightContact, TagSenseContactFeel>().WithEntityAccess())
            {
                var entityReceiver = contact.ValueRO.Receiver;
                var entitySource = contact.ValueRO.Source;

                var receiverComponent = SystemAPI.GetComponentRO<ComponentSenseReceiver>(entityReceiver);
                var sourceComponent = SystemAPI.GetComponentRO<ComponentSenseSource>(entitySource);

                var receiver = SystemAPI.GetComponentRO<ComponentSenseSightReceiver>(entityReceiver).ValueRO;
                var receiverTransform = SystemAPI.GetComponentRO<LocalToWorld>(receiverComponent.ValueRO.Transform).ValueRO;
                var sourceTransform = SystemAPI.GetComponentRO<LocalToWorld>(sourceComponent.ValueRO.Transform).ValueRO;

                if (IsInSightCone(receiverTransform, receiver, sourceTransform.Position, true)
                    && IsRayConnectsDirectly(receiverComponent.ValueRO.Owner, receiverTransform, receiver,
                        sourceComponent.ValueRO.Owner, sourceTransform.Position, ref collisionWorld))
                {
                    continue;
                }

                buffer.SetComponentEnabled<TagSenseContactFeel>(entity, false);

                var eventUpdate = buffer.CreateEntity();
                buffer.AddComponent(eventUpdate, new EventSenseContactUpdateFeel
                {
                    Entity = entity,
                });
                buffer.AddComponent(eventUpdate, new EventSenseSightContactUpdateFeel());
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
            Entity entityReceiverOwner, in LocalToWorld receiverTransform, in ComponentSenseSightReceiver receiver,
            Entity entitySourceOwner, in float3 sourcePosition, ref CollisionWorld collisionWorld)
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
                if (hit.Entity != entityReceiverOwner && hit.Entity != entitySourceOwner)
                {
                    return false;
                }
            }

            return true;
        }
    }
}