using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace ECSPerception
{
    [BurstCompile, UpdateInGroup(typeof(GroupSenseCreateReceiver), OrderFirst = true), UpdateAfter(typeof(SystemSenseReceiverCreateEntity))]
    public partial struct SystemSenseReceiverCreateComponents : ISystem
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

            foreach (var eventCreate in SystemAPI.Query<RefRO<EventSenseReceiverCreate>>())
            {
                var entity = eventCreate.ValueRO.Entity;
                commands.AddComponent(entity, new ComponentSenseReceiver
                {
                    Owner = eventCreate.ValueRO.Owner,
                    Transform = eventCreate.ValueRO.Transform,
                });
                commands.AddBuffer<BufferSenseContact>(entity);
                if (eventCreate.ValueRO.RememberTime > 0)
                {
                    commands.AddComponent(entity, new ComponentSenseReceiverRemember
                    {
                        RememberTime = eventCreate.ValueRO.RememberTime,
                    });
                }
            }

            commands.Playback(state.EntityManager);
        }
    }
}