using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace PerceptionECS
{
    [BurstCompile, UpdateInGroup(typeof(GroupSenseCreateLinecast))]
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
            var buffer = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (eventCreate, _) in
                     SystemAPI.Query<RefRO<EventSenseLinecastCreate>, RefRO<EventSenseSightLinecastCreate>>())
            {
                var contact = SystemAPI.GetComponentRO<ComponentSenseContact>(eventCreate.ValueRO.Contact);
                var (entityReceiver, entitySource) = contact.ValueRO;

                var receiverComponent = SystemAPI.GetComponentRO<ComponentSenseReceiver>(entityReceiver);
                var sourceComponent = SystemAPI.GetComponentRO<ComponentSenseSource>(entitySource);

                var receiver = SystemAPI.GetComponentRO<ComponentSenseSightReceiver>(entityReceiver).ValueRO;
                var receiverTransform = SystemAPI.GetComponentRO<LocalToWorld>(receiverComponent.ValueRO.Transform).ValueRO;

                buffer.AddComponent(eventCreate.ValueRO.Entity, new ComponentSenseLinecast
                {
                    Owner = eventCreate.ValueRO.Contact,
                    ReceiverTransform = receiverComponent.ValueRO.Transform,
                    ReceiverOwner = receiverComponent.ValueRO.Owner,
                    ReceiverOffset = receiverTransform.Forward * -receiver.BackwardOffset,
                    SourceTransform = sourceComponent.ValueRO.Transform,
                    SourceOwner = sourceComponent.ValueRO.Owner,
                    SourceOffset = float3.zero,
                });
            }

            buffer.Playback(state.EntityManager);
        }
    }
}