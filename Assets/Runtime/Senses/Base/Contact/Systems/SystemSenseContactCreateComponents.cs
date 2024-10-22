using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace ECSPerception
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
            var commands = new EntityCommandBuffer(Allocator.Temp);

            foreach (var eventCreate in SystemAPI.Query<RefRO<EventSenseContactCreate>>())
            {
                var entity = eventCreate.ValueRO.Entity;
                var entityReceiver = eventCreate.ValueRO.Receiver;
                var entitySource = eventCreate.ValueRO.Source;

                commands.AddComponent(entity, new ComponentSenseContact
                {
                    Receiver = entityReceiver,
                    Source = entitySource,
                });
                commands.AddComponent(entity, new TagSenseContactFeel());
                commands.SetComponentEnabled<TagSenseContactFeel>(entity, false);
                if (SystemAPI.HasComponent<ComponentSenseReceiverRemember>(entityReceiver))
                {
                    commands.AddComponent(entity, new ComponentSenseContactRemember());
                    commands.AddComponent(entity, new TagSenseContactRemember());
                    commands.SetComponentEnabled<TagSenseContactRemember>(entity, false);
                }
            }

            commands.Playback(state.EntityManager);
        }
    }
}