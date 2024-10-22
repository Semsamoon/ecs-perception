using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace ECSPerception
{
    [BurstCompile, UpdateInGroup(typeof(GroupSenseCreateContact), OrderFirst = true)]
    public partial struct SystemSenseContactCreateEntity : ISystem
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

            foreach (var eventCreate in SystemAPI.Query<RefRW<EventSenseContactCreate>>())
            {
                var entity = state.EntityManager.CreateEntity();
                eventCreate.ValueRW.Entity = entity;

                commands.AppendToBuffer(eventCreate.ValueRO.Receiver, new BufferSenseContact
                {
                    Entity = entity,
                });
                commands.AppendToBuffer(eventCreate.ValueRO.Source, new BufferSenseContact
                {
                    Entity = entity,
                });
            }

            commands.Playback(state.EntityManager);
        }
    }
}