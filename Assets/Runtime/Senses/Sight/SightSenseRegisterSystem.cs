using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace PerceptionECS
{
    [BurstCompile]
    [UpdateBefore(typeof(SightSenseVisibilitySystem))]
    public partial struct SightSenseRegisterSystem : ISystem
    {
        private EntityArchetype _queryArchetype;

        public void OnCreate(ref SystemState state)
        {
            var queryTypes = new NativeArray<ComponentType>(3, Allocator.Temp);
            queryTypes[0] = typeof(SightSenseQueryComponent);
            queryTypes[1] = typeof(SightSenseVisibilityTag);
            queryTypes[2] = typeof(SightSenseRememberTag);
            _queryArchetype = state.EntityManager.CreateArchetype(queryTypes);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var buffer = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (_, entityListener) in
                     SystemAPI
                         .Query<SightSenseListenerComponent>()
                         .WithAll<SightSenseListenerRegisterTag>()
                         .WithEntityAccess())
            {
                foreach (var (_, entitySource) in
                         SystemAPI
                             .Query<SightSenseSourceTag>()
                             .WithDisabled<SightSenseSourceRegisterTag>()
                             .WithEntityAccess())
                {
                    if (entitySource == entityListener) continue;

                    var entityQuery = buffer.CreateEntity(_queryArchetype);
                    buffer.SetComponent(entityQuery, new SightSenseQueryComponent
                    {
                        Listener = entityListener, Source = entitySource, SourcePosition = float3.zero, RememberTime = 0,
                    });
                    buffer.SetComponentEnabled<SightSenseVisibilityTag>(entityQuery, false);
                    buffer.SetComponentEnabled<SightSenseRememberTag>(entityQuery, false);
                }

                buffer.SetComponentEnabled<SightSenseListenerRegisterTag>(entityListener, false);
            }

            buffer.Playback(state.EntityManager);

            buffer = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (_, entitySource) in
                     SystemAPI
                         .Query<SightSenseSourceTag>()
                         .WithAll<SightSenseSourceRegisterTag>()
                         .WithEntityAccess())
            {
                foreach (var (_, entityListener) in
                         SystemAPI
                             .Query<SightSenseListenerComponent>()
                             .WithDisabled<SightSenseListenerRegisterTag>()
                             .WithEntityAccess())
                {
                    if (entityListener == entitySource) continue;

                    var entityQuery = buffer.CreateEntity(_queryArchetype);
                    buffer.SetComponent(entityQuery, new SightSenseQueryComponent
                    {
                        Listener = entityListener, Source = entitySource, SourcePosition = float3.zero, RememberTime = 0,
                    });
                    buffer.SetComponentEnabled<SightSenseVisibilityTag>(entityQuery, false);
                    buffer.SetComponentEnabled<SightSenseRememberTag>(entityQuery, false);
                }

                buffer.SetComponentEnabled<SightSenseSourceRegisterTag>(entitySource, false);
            }

            buffer.Playback(state.EntityManager);
        }
    }
}