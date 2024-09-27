using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace PerceptionECS
{
    [BurstCompile]
    [UpdateBefore(typeof(SystemSenseSight))]
    public partial struct SightSenseRegisterSystem : ISystem
    {
        private EntityArchetype _interactionArchetype;

        public void OnCreate(ref SystemState state)
        {
            var queryTypes = new NativeArray<ComponentType>(6, Allocator.Temp);
            queryTypes[0] = typeof(ComponentSenseInteraction);
            queryTypes[1] = typeof(ComponentSenseInteractionRemember);
            queryTypes[2] = typeof(TagSenseNeglect);
            queryTypes[3] = typeof(TagSenseFeel);
            queryTypes[4] = typeof(TagSenseRemember);
            queryTypes[5] = typeof(TagSenseSightInteraction);
            _interactionArchetype = state.EntityManager.CreateArchetype(queryTypes);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var buffer = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (_, entityReceiver) in
                     SystemAPI
                         .Query<ComponentSenseSightReceiver>()
                         .WithAll<SightSenseListenerRegisterTag>()
                         .WithEntityAccess())
            {
                foreach (var (_, entitySource) in
                         SystemAPI
                             .Query<SightSenseSourceTag>()
                             .WithDisabled<SightSenseSourceRegisterTag>()
                             .WithEntityAccess())
                {
                    if (entitySource == entityReceiver)
                    {
                        continue;
                    }

                    var entityInteraction = buffer.CreateEntity(_interactionArchetype);
                    buffer.SetComponent(entityInteraction, new ComponentSenseInteraction
                    {
                        Receiver = entityReceiver, Source = entitySource
                    });
                    buffer.SetComponentEnabled<TagSenseNeglect>(entityInteraction, true);
                    buffer.SetComponentEnabled<TagSenseFeel>(entityInteraction, false);
                    buffer.SetComponentEnabled<TagSenseRemember>(entityInteraction, false);
                }

                buffer.SetComponentEnabled<SightSenseListenerRegisterTag>(entityReceiver, false);
            }

            buffer.Playback(state.EntityManager);

            buffer = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (_, entitySource) in
                     SystemAPI
                         .Query<SightSenseSourceTag>()
                         .WithAll<SightSenseSourceRegisterTag>()
                         .WithEntityAccess())
            {
                foreach (var (_, entityReceiver) in
                         SystemAPI
                             .Query<ComponentSenseSightReceiver>()
                             .WithDisabled<SightSenseListenerRegisterTag>()
                             .WithEntityAccess())
                {
                    if (entityReceiver == entitySource)
                    {
                        continue;
                    }

                    var entityInteraction = buffer.CreateEntity(_interactionArchetype);
                    buffer.SetComponent(entityInteraction, new ComponentSenseInteraction
                    {
                        Receiver = entityReceiver, Source = entitySource
                    });
                    buffer.SetComponentEnabled<TagSenseFeel>(entityInteraction, false);
                    buffer.SetComponentEnabled<TagSenseRemember>(entityInteraction, false);
                    buffer.SetComponentEnabled<TagSenseNeglect>(entityInteraction, true);
                }

                buffer.SetComponentEnabled<SightSenseSourceRegisterTag>(entitySource, false);
            }

            buffer.Playback(state.EntityManager);
        }
    }
}