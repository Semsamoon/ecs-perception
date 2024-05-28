using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace PerceptionECS
{
    [BurstCompile]
    public partial struct SightSenseUnregisterSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var buffer = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (_, entityListener) in
                     SystemAPI
                         .Query<SightSenseListenerUnregisterTag>()
                         .WithEntityAccess())
            {
                foreach (var (query, entityQuery) in
                         SystemAPI
                             .Query<SightSenseQueryComponent>()
                             .WithEntityAccess())
                {
                    if (query.Listener != entityListener) continue;

                    buffer.DestroyEntity(entityQuery);
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
                foreach (var (query, entityQuery) in
                         SystemAPI
                             .Query<SightSenseQueryComponent>()
                             .WithEntityAccess())
                {
                    if (query.Source != entitySource) continue;

                    buffer.DestroyEntity(entityQuery);
                }

                buffer.SetComponentEnabled<SightSenseSourceUnregisterTag>(entitySource, false);
            }

            buffer.Playback(state.EntityManager);
        }
    }
}