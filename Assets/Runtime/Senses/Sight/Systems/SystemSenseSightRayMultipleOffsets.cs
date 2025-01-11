using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace ECSPerception.Sight
{
    [BurstCompile]
    [UpdateInGroup(typeof(SightSenseSystemGroup))]
    public partial struct SystemSenseSightRayMultipleOffsets : ISystem
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
            var raycastsAmount = SystemAPI.GetSingleton<ComponentSenseSightCurrent>().RaycastsAmount;
            var raycastsLimit = SystemAPI.GetSingleton<ComponentSenseSightLimits>().RaycastsAmount;

            if (raycastsAmount >= raycastsLimit)
            {
                return;
            }

            var commands = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (receiverData, receiverTransform, receiver) in SystemAPI
                         .Query<RefRO<ComponentSenseSightReceiver>, RefRO<LocalToWorld>>()
                         .WithAll<TagSenseSightRayMultiple>()
                         .WithAll<BufferSenseSightPossible, BufferSenseSightOffset>()
                         // [BUG] Here must be WithDisabled, but it doesn't work for IBufferElementData
                         .WithNone<BufferSenseSightExecute>()
                         .WithEntityAccess())
            {
                var receiverPosition = receiverTransform.ValueRO.Value.TransformPoint(receiverData.ValueRO.Offset);
                var possibles = SystemAPI.GetBuffer<BufferSenseSightPossible>(receiver);
                var offsets = SystemAPI.GetBuffer<BufferSenseSightOffset>(receiver);

                var i = 0;

                while (i < possibles.Length && raycastsAmount + offsets.Length < raycastsLimit)
                {
                    commands.AppendToBuffer(receiver, new BufferSenseSightExecute
                    {
                        Receiver = receiver,
                        ReceiverPosition = receiverPosition,
                        SourcePosition = possibles[i].SourcePosition,
                        Source = possibles[i].Source,
                        NearClipRadiusSquared = receiverData.ValueRO.NearClipRadiusSquared,
                    });

                    foreach (var offset in offsets)
                    {
                        commands.AppendToBuffer(receiver, new BufferSenseSightExecute
                        {
                            Receiver = receiver,
                            ReceiverPosition = receiverPosition,
                            SourcePosition = possibles[i].SourcePosition + offset.Offset,
                            Source = possibles[i].Source,
                            NearClipRadiusSquared = receiverData.ValueRO.NearClipRadiusSquared,
                        });
                    }

                    i++;
                    raycastsAmount += 1 + offsets.Length;
                }

                if (i == 0)
                {
                    commands.SetComponentEnabled<BufferSenseSightPossible>(receiver, false);
                    continue;
                }

                if (i == possibles.Length)
                {
                    possibles.Clear();
                    commands.SetComponentEnabled<BufferSenseSightPossible>(receiver, false);
                }
                else
                {
                    possibles.RemoveRangeSwapBack(0, i);
                }

                commands.SetComponentEnabled<BufferSenseSightExecute>(receiver, true);

                if (raycastsAmount == raycastsLimit)
                {
                    break;
                }
            }

            SystemAPI.GetSingletonRW<ComponentSenseSightCurrent>().ValueRW.RaycastsAmount = raycastsAmount;

            commands.Playback(state.EntityManager);
        }
    }
}