using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace PerceptionECS
{
    [BurstCompile, UpdateInGroup(typeof(GroupSenseCreate), OrderFirst = true)]
    public partial struct SystemSenseSightReceiverCreate : ISystem
    {
        private EntityArchetype _archetype;

        public void OnCreate(ref SystemState state)
        {
            var types = new NativeArray<ComponentType>(6, Allocator.Temp);
            types[0] = typeof(ComponentSenseContact);
            types[1] = typeof(TagSenseContactFeel);
            types[2] = typeof(TagSenseSight);
            types[3] = typeof(EventSenseCreate);
            types[4] = typeof(EventSenseDestroy);
            types[5] = typeof(EventSenseContactFeel);
            _archetype = state.EntityManager.CreateArchetype(types);
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var buffer = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (receiverBase, receiverEntity) in
                     SystemAPI.Query<ComponentSenseBase>()
                         .WithAll<ComponentSenseSightReceiver, EventSenseCreate>()
                         .WithEntityAccess())
            {
                foreach (var (sourceBase, sourceEntity) in
                         SystemAPI.Query<ComponentSenseBase>()
                             .WithAll<ComponentSenseSightSource>()
                             .WithDisabled<EventSenseCreate, EventSenseDestroy>()
                             .WithEntityAccess())
                {
                    if (receiverBase.Entity == sourceBase.Entity)
                    {
                        continue;
                    }

                    var connect = buffer.CreateEntity(_archetype);
                    buffer.SetComponent(connect, new ComponentSenseContact
                    {
                        Receiver = receiverEntity,
                        Source = sourceEntity,
                    });
                    buffer.SetComponentEnabled<TagSenseContactFeel>(connect, false);
                    buffer.SetComponentEnabled<EventSenseDestroy>(connect, false);
                }

                buffer.SetComponentEnabled<EventSenseCreate>(receiverEntity, false);
            }

            buffer.Playback(state.EntityManager);
        }
    }
}