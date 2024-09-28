using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace PerceptionECS
{
    [BurstCompile, UpdateBefore(typeof(SystemSenseTransition))]
    public partial struct SystemSenseRegister : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var buffer = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (_, entityInteraction) in
                     SystemAPI.Query<TagSenseRegister>().WithEntityAccess())
            {
                buffer.SetComponentEnabled<TagSenseRegister>(entityInteraction, false);
            }

            buffer.Playback(state.EntityManager);
        }
    }
}