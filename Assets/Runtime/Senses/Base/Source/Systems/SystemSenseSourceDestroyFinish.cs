﻿using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace ECSPerception
{
    [BurstCompile, UpdateInGroup(typeof(GroupSenseDestroySource), OrderLast = true), UpdateAfter(typeof(SystemSenseSourceDestroyEntity))]
    public partial struct SystemSenseSourceDestroyFinish : ISystem
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
                         .Query<RefRO<EventSenseSourceDestroy>>()
                         .WithEntityAccess())
            {
                commands.DestroyEntity(entity);
            }

            commands.Playback(state.EntityManager);
        }
    }
}