using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace PerceptionECS
{
    [BurstCompile, UpdateInGroup(typeof(GroupSenseCreateContact), OrderFirst = true), UpdateAfter(typeof(SystemSenseContactCreateEntity))]
    public partial struct SystemSenseContactCreateComponents : ISystem
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

            foreach (var eventCreate in SystemAPI.Query<RefRO<EventSenseContactCreate>>())
            {
                var entity = eventCreate.ValueRO.Entity;
                var entityReceiver = eventCreate.ValueRO.Receiver;
                var entitySource = eventCreate.ValueRO.Source;

                buffer.AddComponent(entity, new ComponentSenseContact
                {
                    Receiver = entityReceiver,
                    Source = entitySource,
                });
                buffer.AddComponent(entity, new TagSenseContactFeel());
                buffer.SetComponentEnabled<TagSenseContactFeel>(entity, false);
                if (SystemAPI.HasComponent<ComponentSenseReceiverRemember>(entityReceiver))
                {
                    buffer.AddComponent(entity, new ComponentSenseContactRemember());
                    buffer.AddComponent(entity, new TagSenseContactRemember());
                    buffer.SetComponentEnabled<TagSenseContactRemember>(entity, false);
                }
            }

            buffer.Playback(state.EntityManager);
        }
    }
}