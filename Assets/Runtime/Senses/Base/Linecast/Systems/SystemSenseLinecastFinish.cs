using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace ECSPerception
{
    [BurstCompile, UpdateInGroup(typeof(GroupSenseUpdateLinecast), OrderLast = true), UpdateAfter(typeof(SystemSenseLinecastResult))]
    public partial struct SystemSenseLinecastFinish : ISystem
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

            foreach (var (_, entity) in
                     SystemAPI.Query<RefRO<ComponentSenseLinecast>>().WithDisabled<TagSenseLinecastWait>().WithEntityAccess())
            {
                buffer.DestroyEntity(entity);
            }

            buffer.Playback(state.EntityManager);
        }
    }
}