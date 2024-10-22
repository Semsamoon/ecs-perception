using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace PerceptionECS
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
            var buffer = new EntityCommandBuffer(Allocator.Temp);

            foreach (var eventCreate in SystemAPI.Query<RefRO<EventSenseReceiverCreate>>())
            {
                var entity = eventCreate.ValueRO.Entity;
                buffer.AddComponent(entity, new ComponentSenseReceiver
                {
                    Owner = eventCreate.ValueRO.Owner,
                    Transform = eventCreate.ValueRO.Transform,
                });
                buffer.AddBuffer<BufferSenseContact>(entity);
                if (eventCreate.ValueRO.RememberTime > 0)
                {
                    buffer.AddComponent(entity, new ComponentSenseReceiverRemember
                    {
                        RememberTime = eventCreate.ValueRO.RememberTime,
                    });
                }
            }

            buffer.Playback(state.EntityManager);
        }
    }
}