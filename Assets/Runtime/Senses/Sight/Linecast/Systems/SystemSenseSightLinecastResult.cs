﻿using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace ECSPerception
{
    [BurstCompile, UpdateInGroup(typeof(GroupSenseUpdateLinecast), OrderLast = true)]
    public partial struct SystemSenseSightLinecastResult : ISystem
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

            foreach (var linecast in SystemAPI
                         .Query<RefRO<ComponentSenseSightLinecast>>()
                         .WithAll<TagSenseLinecastResult>()
                         .WithDisabled<TagSenseLinecastWait>())
            {
                commands.SetComponentEnabled<TagSenseSightContactLinecastResult>(linecast.ValueRO.Contact, true);
            }

            foreach (var linecast in SystemAPI
                         .Query<RefRO<ComponentSenseSightLinecast>>()
                         .WithDisabled<TagSenseLinecastResult, TagSenseLinecastWait>())
            {
                commands.SetComponentEnabled<TagSenseSightContactLinecastResult>(linecast.ValueRO.Contact, false);
            }

            commands.Playback(state.EntityManager);
        }
    }
}