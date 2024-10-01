using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace PerceptionECS
{
    [BurstCompile, UpdateInGroup(typeof(GroupSenseCreateReceiver))]
    public partial struct SystemSenseSightReceiverCreateContacts : ISystem
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

            foreach (var eventCreate in SystemAPI.Query<RefRO<EventSenseReceiverCreate>>().WithAll<EventSenseSightReceiverCreate>())
            {
                var entityReceiver = eventCreate.ValueRO.Entity;
                var entityReceiverOwner = SystemAPI.GetComponentRO<ComponentSenseReceiver>(entityReceiver).ValueRO.Owner;

                foreach (var (source, entitySource) in
                         SystemAPI.Query<RefRO<ComponentSenseSource>>().WithAll<ComponentSenseSightSource>().WithEntityAccess())
                {
                    var entitySourceOwner = source.ValueRO.Owner;

                    if (entityReceiverOwner == entitySourceOwner)
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