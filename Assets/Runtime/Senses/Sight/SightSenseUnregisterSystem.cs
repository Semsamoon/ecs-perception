using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace PerceptionECS
{
    [BurstCompile]
    public partial struct SightSenseUnregisterSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var buffer = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (_, entityListener) in
                     SystemAPI
                         .Query<SightSenseListenerUnregisterTag>()
                         .WithEntityAccess())
            {
                foreach (var (interaction, entityInteraction) in
                         SystemAPI
                             .Query<ComponentSenseInteraction>()
                             .WithEntityAccess())
                {
                    if (interaction.Receiver != entityListener) continue;

                    buffer.DestroyEntity(entityInteraction);
                }

                buffer.SetComponentEnabled<SightSenseListenerUnregisterTag>(entityListener, false);
            }

            buffer.Playback(state.EntityManager);

            buffer = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (_, entitySource) in
                     SystemAPI
                         .Query<SightSenseSourceUnregisterTag>()
                         .WithEntityAccess())
            {
                foreach (var (interaction, entityInteraction) in
                         SystemAPI
                             .Query<ComponentSenseInteraction>()
                             .WithEntityAccess())
                {
                    if (interaction.Source != entitySource) continue;

                    buffer.DestroyEntity(entityInteraction);
                }

                buffer.SetComponentEnabled<SightSenseSourceUnregisterTag>(entitySource, false);
            }

            buffer.Playback(state.EntityManager);
        }
    }
}