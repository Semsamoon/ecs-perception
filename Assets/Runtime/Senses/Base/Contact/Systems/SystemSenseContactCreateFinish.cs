﻿using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace ECSPerception
{
    [BurstCompile, UpdateInGroup(typeof(GroupSenseCreateContact), OrderLast = true)]
    public partial struct SystemSenseContactCreateFinish : ISystem
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

            foreach (var (_, entity) in SystemAPI
                         .Query<RefRO<EventSenseContactCreate>>()
                         .WithEntityAccess())
            {
                commands.DestroyEntity(entity);
            }

            commands.Playback(state.EntityManager);
        }
    }
}