using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace ECSPerception
{
    [BurstCompile, UpdateInGroup(typeof(GroupSenseDestroyContact), OrderLast = true)]
    public partial struct SystemSenseContactDestroyEntity : ISystem
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
            var commands = new EntityCommandBuffer(Allocator.Temp);

            foreach (var eventDestroy in SystemAPI.Query<RefRO<EventSenseContactDestroy>>())
            {
                var contactEntity = eventDestroy.ValueRO.Entity;
                var contact = SystemAPI.GetComponentRO<ComponentSenseContact>(contactEntity);
                var receiver = contact.ValueRO.Receiver;
                var source = contact.ValueRO.Source;

                if (state.EntityManager.Exists(receiver))
                {
                    var contactsReceiver = SystemAPI.GetBuffer<BufferSenseContact>(receiver);

                    for (var i = 0; i < contactsReceiver.Length; i++)
                    {
                        if (contactsReceiver[i].Entity == contactEntity)
                        {
                            contactsReceiver.RemoveAt(i);
                            break;
                        }
                    }
                }

                if (state.EntityManager.Exists(source))
                {
                    var contactsSource = SystemAPI.GetBuffer<BufferSenseContact>(source);

                    for (var i = 0; i < contactsSource.Length; i++)
                    {
                        if (contactsSource[i].Entity == contactEntity)
                        {
                            contactsSource.RemoveAt(i);
                            break;
                        }
                    }
                }

                commands.DestroyEntity(eventDestroy.ValueRO.Entity);
            }

            commands.Playback(state.EntityManager);
        }
    }
}