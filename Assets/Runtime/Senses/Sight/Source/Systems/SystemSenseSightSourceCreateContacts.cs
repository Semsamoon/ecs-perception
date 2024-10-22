using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace ECSPerception
{
    [BurstCompile, UpdateInGroup(typeof(GroupSenseCreateSource))]
    public partial struct SystemSenseSightSourceCreateContacts : ISystem
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

            foreach (var eventCreate in SystemAPI.Query<RefRO<EventSenseSourceCreate>>().WithAll<EventSenseSightSourceCreate>())
            {
                var entitySource = eventCreate.ValueRO.Entity;
                var entitySourceOwner = SystemAPI.GetComponentRO<ComponentSenseSource>(entitySource).ValueRO.Owner;

                foreach (var (receiver, entityReceiver) in
                         SystemAPI.Query<RefRO<ComponentSenseReceiver>>().WithAll<ComponentSenseSightReceiver>().WithEntityAccess())
                {
                    var entityReceiverOwner = receiver.ValueRO.Owner;

                    if (entitySourceOwner == entityReceiverOwner)
                    {
                        continue;
                    }

                    var eventContactCreate = buffer.CreateEntity();
                    buffer.AddComponent(eventContactCreate, new EventSenseContactCreate
                    {
                        Receiver = entityReceiver,
                        Source = entitySource,
                    });
                    buffer.AddComponent(eventContactCreate, new EventSenseSightContactCreate());
                }
            }

            buffer.Playback(state.EntityManager);
        }
    }
}