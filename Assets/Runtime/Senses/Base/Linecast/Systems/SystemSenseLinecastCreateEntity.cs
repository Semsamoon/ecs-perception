using Unity.Burst;
using Unity.Entities;

namespace ECSPerception
{
    [BurstCompile, UpdateInGroup(typeof(GroupSenseCreateLinecast), OrderFirst = true)]
    public partial struct SystemSenseLinecastCreateEntity : ISystem
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
            foreach (var eventCreate in SystemAPI.Query<RefRW<EventSenseLinecastCreate>>())
            {
                var entity = state.EntityManager.CreateEntity();
                eventCreate.ValueRW.Entity = entity;
            }
        }
    }
}