using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace PerceptionECS
{
    [BurstCompile, UpdateAfter(typeof(SystemSenseRegister)), UpdateBefore(typeof(SystemSenseTransition))]
    public partial struct SystemSenseRegisterRemember : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var buffer = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (interaction, entityInteraction) in
                     SystemAPI.Query<ComponentSenseInteraction>().WithAll<TagSenseRegister>().WithEntityAccess())
            {
                if (!SystemAPI.HasComponent<ComponentSenseReceiverRemember>(interaction.Receiver))
                {
                    continue;
                }

                buffer.AddComponent<ComponentSenseRemember>(entityInteraction);
                buffer.AddComponent<TagSenseRemember>(entityInteraction);
                buffer.SetComponentEnabled<TagSenseRemember>(entityInteraction, false);
            }

            buffer.Playback(state.EntityManager);
        }
    }
}