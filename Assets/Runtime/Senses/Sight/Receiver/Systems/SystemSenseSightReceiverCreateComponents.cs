using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace ECSPerception
{
    [BurstCompile, UpdateInGroup(typeof(GroupSenseCreateReceiver))]
    public partial struct SystemSenseSightReceiverCreateComponents : ISystem
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

            foreach (var (eventCreate, eventSightCreate) in SystemAPI
                         .Query<RefRO<EventSenseReceiverCreate>, RefRO<EventSenseSightReceiverCreate>>())
            {
                commands.AddComponent(eventCreate.ValueRO.Entity, new ComponentSenseSightReceiver
                {
                    ViewAngleCos = eventSightCreate.ValueRO.ViewAngleCos,
                    ViewRadiusSquared = eventSightCreate.ValueRO.ViewRadiusSquared,
                    LoseRadiusSquared = eventSightCreate.ValueRO.LoseRadiusSquared,
                    BackwardOffset = eventSightCreate.ValueRO.BackwardOffset,
                    NearClipRadiusSquared = eventSightCreate.ValueRO.NearClipRadiusSquared,
                });
            }

            commands.Playback(state.EntityManager);
        }
    }
}