using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace PerceptionECS
{
    [BurstCompile, UpdateInGroup(typeof(GroupSenseDestroySource), OrderFirst = true)]
    public partial struct SystemSenseSightSourceDestroyContacts : ISystem
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

            foreach (var eventDestroy in SystemAPI.Query<RefRO<EventSenseSourceDestroy>>().WithAll<EventSenseSightSourceDestroy>())
            {
                var entitySource = eventDestroy.ValueRO.Entity;

                foreach (var (contact, entityContact) in
                         SystemAPI.Query<RefRO<ComponentSenseContact>>().WithAll<ComponentSenseSightContact>().WithEntityAccess())
                {
                    if (contact.ValueRO.Source != entitySource)
                    {
                        continue;
                    }

                    var eventContactDestroy = buffer.CreateEntity();
                    buffer.AddComponent(eventContactDestroy, new EventSenseContactDestroy
                    {
                        Entity = entityContact,
                    });
                    buffer.AddComponent(eventContactDestroy, new EventSenseSightContactDestroy());
                }
            }

            buffer.Playback(state.EntityManager);
        }
    }
}