﻿using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace PerceptionECS
{
    [BurstCompile, UpdateInGroup(typeof(GroupSenseDestroyReceiver), OrderFirst = true)]
    public partial struct SystemSenseSightReceiverDestroyContacts : ISystem
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

            foreach (var eventDestroy in SystemAPI.Query<RefRO<EventSenseReceiverDestroy>>().WithAll<EventSenseSightReceiverDestroy>())
            {
                var entityReceiver = eventDestroy.ValueRO.Entity;

                foreach (var (contact, entityContact) in
                         SystemAPI.Query<RefRO<ComponentSenseContact>>().WithAll<ComponentSenseSightContact>().WithEntityAccess())
                {
                    if (contact.ValueRO.Receiver != entityReceiver)
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