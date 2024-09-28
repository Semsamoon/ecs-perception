using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace PerceptionECS
{
    [BurstCompile, UpdateAfter(typeof(SystemSenseRegister)), UpdateBefore(typeof(SystemSenseRegisterRemember))]
    public partial struct SystemSenseSightRegisterInteraction : ISystem
    {
        private EntityArchetype _archetype;

        public void OnCreate(ref SystemState state)
        {
            var baseSightSenseTypes = new NativeArray<ComponentType>(7, Allocator.Temp);
            baseSightSenseTypes[0] = typeof(ComponentSenseInteraction);
            baseSightSenseTypes[1] = typeof(TagSenseSightInteraction);
            baseSightSenseTypes[2] = typeof(TagSenseNeglect);
            baseSightSenseTypes[3] = typeof(TagSenseFeel);
            baseSightSenseTypes[4] = typeof(TagSenseTransitionNeglectToFeel);
            baseSightSenseTypes[5] = typeof(TagSenseTransitionFeelToNeglect);
            baseSightSenseTypes[6] = typeof(TagSenseRegister);
            _archetype = state.EntityManager.CreateArchetype(baseSightSenseTypes);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var buffer = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (_, entityReceiver) in
                     SystemAPI.Query<TagSenseRegisterReceiver>().WithAll<ComponentSenseSightReceiver>().WithEntityAccess())
            {
                foreach (var (_, entitySource) in
                         SystemAPI.Query<RefRO<ComponentSenseSightSource>>().WithDisabled<TagSenseRegisterSource>().WithEntityAccess())
                {
                    if (entitySource == entityReceiver)
                    {
                        continue;
                    }

                    var entityInteraction = buffer.CreateEntity(_archetype);
                    buffer.SetComponent(entityInteraction, new ComponentSenseInteraction
                    {
                        Receiver = entityReceiver, Source = entitySource
                    });
                    buffer.SetComponentEnabled<TagSenseFeel>(entityInteraction, false);
                    buffer.SetComponentEnabled<TagSenseTransitionNeglectToFeel>(entityInteraction, false);
                    buffer.SetComponentEnabled<TagSenseTransitionFeelToNeglect>(entityInteraction, false);
                    // buffer.SetComponentEnabled<TagSenseRemember>(entityInteraction, false);
                }

                buffer.SetComponentEnabled<TagSenseRegisterReceiver>(entityReceiver, false);
            }

            buffer.Playback(state.EntityManager);

            buffer = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (source, entitySource) in
                     SystemAPI.Query<TagSenseRegisterSource>().WithAll<ComponentSenseSightSource>().WithEntityAccess())
            {
                foreach (var (_, entityReceiver) in
                         SystemAPI.Query<RefRO<ComponentSenseSightReceiver>>().WithDisabled<TagSenseRegisterReceiver>().WithEntityAccess())
                {
                    if (entityReceiver == entitySource)
                    {
                        continue;
                    }

                    var entityInteraction = buffer.CreateEntity(_archetype);
                    buffer.SetComponent(entityInteraction, new ComponentSenseInteraction
                    {
                        Receiver = entityReceiver, Source = entitySource
                    });
                    buffer.SetComponentEnabled<TagSenseFeel>(entityInteraction, false);
                    buffer.SetComponentEnabled<TagSenseTransitionNeglectToFeel>(entityInteraction, false);
                    buffer.SetComponentEnabled<TagSenseTransitionFeelToNeglect>(entityInteraction, false);
                    // buffer.SetComponentEnabled<TagSenseRemember>(entityInteraction, false);
                }

                buffer.SetComponentEnabled<TagSenseRegisterSource>(entitySource, false);
            }

            buffer.Playback(state.EntityManager);
        }
    }
}