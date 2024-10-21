using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
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

            foreach (var (contact, entity) in
                     SystemAPI.Query<RefRO<ComponentSenseContact>>().WithAll<ComponentSenseSightContact>()
                         .WithDisabled<TagSenseContactFeel, TagSenseContactLineCast, TagSenseContactLineUp>().WithEntityAccess())
            {
                var (entityReceiver, entitySource) = contact.ValueRO;

                var receiverComponent = SystemAPI.GetComponentRO<ComponentSenseReceiver>(entityReceiver);
                var sourceComponent = SystemAPI.GetComponentRO<ComponentSenseSource>(entitySource);

                var receiver = SystemAPI.GetComponentRO<ComponentSenseSightReceiver>(entityReceiver).ValueRO;
                var receiverTransform = SystemAPI.GetComponentRO<LocalToWorld>(receiverComponent.ValueRO.Transform).ValueRO;
                var sourceTransform = SystemAPI.GetComponentRO<LocalToWorld>(sourceComponent.ValueRO.Transform).ValueRO;

                if (!IsInSightCone(receiverTransform, receiver, sourceTransform.Position, false))
                {
                    continue;
                }

                var linecastEntity = buffer.CreateEntity();
                buffer.AddComponent(linecastEntity, new ComponentSenseLinecast
                {
                    Owner = entity,
                    ReceiverTransform = receiverComponent.ValueRO.Transform,
                    ReceiverOwner = receiverComponent.ValueRO.Owner,
                    ReceiverOffset = receiverTransform.Forward * -receiver.BackwardOffset,
                    SourceTransform = sourceComponent.ValueRO.Transform,
                    SourceOwner = sourceComponent.ValueRO.Owner,
                    SourceOffset = float3.zero,
                });
                buffer.AddComponent(linecastEntity, new TagSenseLinecastWait());
                buffer.AddComponent(linecastEntity, new TagSenseLinecastSuccess());
                buffer.SetComponentEnabled<TagSenseLinecastSuccess>(linecastEntity, false);

                buffer.SetComponentEnabled<TagSenseContactLineCast>(entity, true);
            }

            foreach (var (contact, entity) in
                     SystemAPI.Query<RefRO<ComponentSenseContact>>().WithAll<ComponentSenseSightContact, TagSenseContactLineUp>()
                         .WithDisabled<TagSenseContactFeel, TagSenseContactLineCast>().WithEntityAccess())
            {
                buffer.SetComponentEnabled<TagSenseContactLineUp>(entity, false);

                var (entityReceiver, entitySource) = contact.ValueRO;

                var receiverComponent = SystemAPI.GetComponentRO<ComponentSenseReceiver>(entityReceiver);
                var sourceComponent = SystemAPI.GetComponentRO<ComponentSenseSource>(entitySource);

                var receiver = SystemAPI.GetComponentRO<ComponentSenseSightReceiver>(entityReceiver).ValueRO;
                var receiverTransform = SystemAPI.GetComponentRO<LocalToWorld>(receiverComponent.ValueRO.Transform).ValueRO;
                var sourceTransform = SystemAPI.GetComponentRO<LocalToWorld>(sourceComponent.ValueRO.Transform).ValueRO;

                if (!IsInSightCone(receiverTransform, receiver, sourceTransform.Position, false))
                {
                    continue;
                }

                var linecastEntity = buffer.CreateEntity();
                buffer.AddComponent(linecastEntity, new ComponentSenseLinecast
                {
                    Owner = entity,
                    ReceiverTransform = receiverComponent.ValueRO.Transform,
                    ReceiverOwner = receiverComponent.ValueRO.Owner,
                    ReceiverOffset = receiverTransform.Forward * -receiver.BackwardOffset,
                    SourceTransform = sourceComponent.ValueRO.Transform,
                    SourceOwner = sourceComponent.ValueRO.Owner,
                    SourceOffset = float3.zero,
                });
                buffer.AddComponent(linecastEntity, new TagSenseLinecastWait());
                buffer.AddComponent(linecastEntity, new TagSenseLinecastSuccess());
                buffer.SetComponentEnabled<TagSenseLinecastSuccess>(linecastEntity, false);

                buffer.SetComponentEnabled<TagSenseContactLineCast>(entity, true);
                buffer.SetComponentEnabled<TagSenseContactFeel>(entity, true);

                var eventUpdate = buffer.CreateEntity();
                buffer.AddComponent(eventUpdate, new EventSenseContactUpdateFeel
                {
                    Entity = entity,
                });
                buffer.AddComponent(eventUpdate, new EventSenseSightContactUpdateFeel());
            }

            foreach (var (contact, entity) in
                     SystemAPI.Query<RefRO<ComponentSenseContact>>().WithAll<ComponentSenseSightContact, TagSenseContactFeel>()
                         .WithDisabled<TagSenseContactLineCast, TagSenseContactLineUp>().WithEntityAccess())
            {
                buffer.SetComponentEnabled<TagSenseContactFeel>(entity, false);

                var eventUpdate = buffer.CreateEntity();
                buffer.AddComponent(eventUpdate, new EventSenseContactUpdateFeel
                {
                    Entity = entity,
                });
                buffer.AddComponent(eventUpdate, new EventSenseSightContactUpdateFeel());

                var (entityReceiver, entitySource) = contact.ValueRO;

                var receiverComponent = SystemAPI.GetComponentRO<ComponentSenseReceiver>(entityReceiver);
                var sourceComponent = SystemAPI.GetComponentRO<ComponentSenseSource>(entitySource);

                var receiver = SystemAPI.GetComponentRO<ComponentSenseSightReceiver>(entityReceiver).ValueRO;
                var receiverTransform = SystemAPI.GetComponentRO<LocalToWorld>(receiverComponent.ValueRO.Transform).ValueRO;
                var sourceTransform = SystemAPI.GetComponentRO<LocalToWorld>(sourceComponent.ValueRO.Transform).ValueRO;

                if (!IsInSightCone(receiverTransform, receiver, sourceTransform.Position, true))
                {
                    continue;
                }

                var linecastEntity = buffer.CreateEntity();
                buffer.AddComponent(linecastEntity, new ComponentSenseLinecast
                {
                    Owner = entity,
                    ReceiverTransform = receiverComponent.ValueRO.Transform,
                    ReceiverOwner = receiverComponent.ValueRO.Owner,
                    ReceiverOffset = receiverTransform.Forward * -receiver.BackwardOffset,
                    SourceTransform = sourceComponent.ValueRO.Transform,
                    SourceOwner = sourceComponent.ValueRO.Owner,
                    SourceOffset = float3.zero,
                });
                buffer.AddComponent(linecastEntity, new TagSenseLinecastWait());
                buffer.AddComponent(linecastEntity, new TagSenseLinecastSuccess());
                buffer.SetComponentEnabled<TagSenseLinecastSuccess>(linecastEntity, false);

                buffer.SetComponentEnabled<TagSenseContactLineCast>(entity, true);
            }

            foreach (var (contact, entity) in
                     SystemAPI.Query<RefRO<ComponentSenseContact>>()
                         .WithAll<ComponentSenseSightContact, TagSenseContactFeel, TagSenseContactLineCast>()
                         .WithDisabled<TagSenseContactLineUp>().WithEntityAccess())
            {
                var (entityReceiver, entitySource) = contact.ValueRO;

                var receiverComponent = SystemAPI.GetComponentRO<ComponentSenseReceiver>(entityReceiver);
                var sourceComponent = SystemAPI.GetComponentRO<ComponentSenseSource>(entitySource);

                var receiver = SystemAPI.GetComponentRO<ComponentSenseSightReceiver>(entityReceiver).ValueRO;
                var receiverTransform = SystemAPI.GetComponentRO<LocalToWorld>(receiverComponent.ValueRO.Transform).ValueRO;
                var sourceTransform = SystemAPI.GetComponentRO<LocalToWorld>(sourceComponent.ValueRO.Transform).ValueRO;

                if (IsInSightCone(receiverTransform, receiver, sourceTransform.Position, true))
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

            foreach (var (contact, entity) in
                     SystemAPI.Query<RefRO<ComponentSenseContact>>()
                         .WithAll<ComponentSenseSightContact, TagSenseContactFeel, TagSenseContactLineUp>()
                         .WithDisabled<TagSenseContactLineCast>().WithEntityAccess())
            {
                buffer.SetComponentEnabled<TagSenseContactLineUp>(entity, false);

                var (entityReceiver, entitySource) = contact.ValueRO;

                var receiverComponent = SystemAPI.GetComponentRO<ComponentSenseReceiver>(entityReceiver);
                var sourceComponent = SystemAPI.GetComponentRO<ComponentSenseSource>(entitySource);

                var receiver = SystemAPI.GetComponentRO<ComponentSenseSightReceiver>(entityReceiver).ValueRO;
                var receiverTransform = SystemAPI.GetComponentRO<LocalToWorld>(receiverComponent.ValueRO.Transform).ValueRO;
                var sourceTransform = SystemAPI.GetComponentRO<LocalToWorld>(sourceComponent.ValueRO.Transform).ValueRO;

                if (!IsInSightCone(receiverTransform, receiver, sourceTransform.Position, true))
                {
                    buffer.SetComponentEnabled<TagSenseContactFeel>(entity, false);

                    var eventUpdate = buffer.CreateEntity();
                    buffer.AddComponent(eventUpdate, new EventSenseContactUpdateFeel
                    {
                        Entity = entity,
                    });
                    buffer.AddComponent(eventUpdate, new EventSenseSightContactUpdateFeel());

                    continue;
                }

                var linecastEntity = buffer.CreateEntity();
                buffer.AddComponent(linecastEntity, new ComponentSenseLinecast
                {
                    Owner = entity,
                    ReceiverTransform = receiverComponent.ValueRO.Transform,
                    ReceiverOwner = receiverComponent.ValueRO.Owner,
                    ReceiverOffset = receiverTransform.Forward * -receiver.BackwardOffset,
                    SourceTransform = sourceComponent.ValueRO.Transform,
                    SourceOwner = sourceComponent.ValueRO.Owner,
                    SourceOffset = float3.zero,
                });
                buffer.AddComponent(linecastEntity, new TagSenseLinecastWait());
                buffer.AddComponent(linecastEntity, new TagSenseLinecastSuccess());
                buffer.SetComponentEnabled<TagSenseLinecastSuccess>(linecastEntity, false);

                buffer.SetComponentEnabled<TagSenseContactLineCast>(entity, true);
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
    }
}