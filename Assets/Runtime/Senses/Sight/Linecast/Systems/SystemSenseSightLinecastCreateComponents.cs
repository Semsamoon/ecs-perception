using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECSPerception
{
    [BurstCompile, UpdateInGroup(typeof(GroupSenseCreateLinecast)), UpdateAfter(typeof(SystemSenseSightLinecastCreateEntity))]
    public partial struct SystemSenseSightLinecastCreateComponents : ISystem
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
            var commands = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (eventCreate, eventSightCreate) in
                     SystemAPI.Query<RefRO<EventSenseLinecastCreate>, RefRO<EventSenseSightLinecastCreate>>())
            {
                var contact = SystemAPI.GetComponentRO<ComponentSenseContact>(eventSightCreate.ValueRO.Contact);
                var (entityReceiver, entitySource) = contact.ValueRO;

                var receiverComponent = SystemAPI.GetComponentRO<ComponentSenseReceiver>(entityReceiver);
                var sourceComponent = SystemAPI.GetComponentRO<ComponentSenseSource>(entitySource);

                var receiver = SystemAPI.GetComponentRO<ComponentSenseSightReceiver>(entityReceiver).ValueRO;
                var receiverTransform = SystemAPI.GetComponentRO<LocalToWorld>(receiverComponent.ValueRO.Transform).ValueRO;

                commands.AddComponent(eventCreate.ValueRO.Entity, new ComponentSenseLinecast
                {
                    ReceiverTransform = receiverComponent.ValueRO.Transform,
                    ReceiverOwner = receiverComponent.ValueRO.Owner,
                    ReceiverOffset = receiverTransform.Forward * -receiver.BackwardOffset,
                    SourceTransform = sourceComponent.ValueRO.Transform,
                    SourceOwner = sourceComponent.ValueRO.Owner,
                    SourceOffset = float3.zero,
                });
                commands.AddComponent(eventCreate.ValueRO.Entity, new ComponentSenseSightLinecast
                {
                    Contact = eventSightCreate.ValueRO.Contact,
                });
            }

            commands.Playback(state.EntityManager);
        }
    }
}