using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace ECSPerception.Sight
{
    [BurstCompile]
    [UpdateInGroup(typeof(SightSenseSystemGroup))]
    public partial struct SystemSenseSightCastRaySingle : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var raycastsLimit = SystemAPI.GetSingleton<ComponentSenseSightSettings>().RaycastsLimit;
            var raycastsAmount = SystemAPI.GetSingleton<ComponentSenseSightState>().RaycastsAmount;

            if (raycastsAmount >= raycastsLimit)
            {
                return;
            }

            var commands = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (receiverData, receiverTransform, receiver) in SystemAPI
                         .Query<RefRO<ComponentSenseSightReceiver>, RefRO<LocalToWorld>>()
                         .WithAll<TagSenseSightSingleCast>()
                         .WithAll<BufferSenseSightCastNeed>()
                         // [BUG] Here must be WithDisabled, but it doesn't work for IBufferElementData
                         .WithNone<BufferSenseSightCastExecute>()
                         .WithEntityAccess())
            {
                var receiverPosition = receiverTransform.ValueRO.Value.TransformPoint(receiverData.ValueRO.Offset);
                var needs = SystemAPI.GetBuffer<BufferSenseSightCastNeed>(receiver);

                var i = 0;

                while (i < needs.Length && raycastsAmount < raycastsLimit)
                {
                    commands.AppendToBuffer(receiver, new BufferSenseSightCastExecute
                    {
                        Receiver = receiver,
                        ReceiverPosition = receiverPosition,
                        SourcePosition = needs[i].SourcePosition,
                        Source = needs[i].Source,
                        NearClipRadiusSquared = receiverData.ValueRO.NearClipRadiusSquared,
                    });

                    i++;
                    raycastsAmount++;
                }

                if (i == 0)
                {
                    commands.SetComponentEnabled<BufferSenseSightCastNeed>(receiver, false);
                    continue;
                }

                if (i == needs.Length)
                {
                    needs.Clear();
                    commands.SetComponentEnabled<BufferSenseSightCastNeed>(receiver, false);
                }
                else
                {
                    needs.RemoveRangeSwapBack(0, i);
                }

                commands.SetComponentEnabled<BufferSenseSightCastExecute>(receiver, true);

                if (raycastsAmount == raycastsLimit)
                {
                    break;
                }
            }

            SystemAPI.GetSingletonRW<ComponentSenseSightState>().ValueRW.RaycastsAmount = raycastsAmount;

            commands.Playback(state.EntityManager);
        }
    }
}