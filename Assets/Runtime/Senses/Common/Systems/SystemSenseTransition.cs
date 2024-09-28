using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace PerceptionECS
{
    [BurstCompile, UpdateBefore(typeof(SystemSenseRemember))]
    public partial struct SystemSenseTransition : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var buffer = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (_, entityInteraction) in
                     SystemAPI.Query<TagSenseTransitionNeglectToFeel>().WithEntityAccess())
            {
                buffer.SetComponentEnabled<TagSenseTransitionNeglectToFeel>(entityInteraction, false);
            }

            foreach (var (_, entityInteraction) in
                     SystemAPI.Query<TagSenseTransitionFeelToNeglect>().WithEntityAccess())
            {
                buffer.SetComponentEnabled<TagSenseTransitionFeelToNeglect>(entityInteraction, false);
            }

            buffer.Playback(state.EntityManager);
        }
    }
}