﻿using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace ECSPerception
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
            var commands = new EntityCommandBuffer(Allocator.Temp);

            foreach (var eventDestroy in SystemAPI
                         .Query<RefRO<EventSenseReceiverDestroy>>()
                         .WithAll<EventSenseSightReceiverDestroy>())
            {
                var entityReceiver = eventDestroy.ValueRO.Entity;
                var contacts = SystemAPI.GetBuffer<BufferSenseContact>(entityReceiver);

                foreach (var contact in contacts)
                {
                    var eventContactDestroy = commands.CreateEntity();
                    commands.AddComponent(eventContactDestroy, new EventSenseContactDestroy
                    {
                        Entity = contact.Entity,
                    });
                    commands.AddComponent(eventContactDestroy, new EventSenseSightContactDestroy());
                }
            }

            commands.Playback(state.EntityManager);
        }
    }
}