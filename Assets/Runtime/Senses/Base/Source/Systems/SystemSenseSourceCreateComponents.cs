using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace ECSPerception
{
    [BurstCompile, UpdateInGroup(typeof(GroupSenseCreateSource), OrderFirst = true), UpdateAfter(typeof(SystemSenseSourceCreateEntity))]
    public partial struct SystemSenseSourceCreateComponents : ISystem
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

            foreach (var eventCreate in SystemAPI.Query<RefRO<EventSenseSourceCreate>>())
            {
                var entity = eventCreate.ValueRO.Entity;
                buffer.AddComponent(entity, new ComponentSenseSource
                {
                    Owner = eventCreate.ValueRO.Owner,
                    Transform = eventCreate.ValueRO.Transform,
                });
                buffer.AddBuffer<BufferSenseContact>(entity);
            }

            buffer.Playback(state.EntityManager);
        }
    }
}