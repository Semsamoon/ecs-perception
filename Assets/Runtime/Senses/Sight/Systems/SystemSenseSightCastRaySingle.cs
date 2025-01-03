using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace ECSPerception.Sight
{
    [BurstCompile]
    [UpdateInGroup(typeof(SightSenseSystemGroup)), UpdateAfter(typeof(SystemSenseSightCastCone))]
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
            var commands = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (receiverData, receiverTransform, receiver) in SystemAPI
                         .Query<RefRO<ComponentSenseSightReceiver>, RefRO<LocalToWorld>>()
                         .WithAll<TagSenseSightSingleCast>()
                         .WithAll<BufferSenseSightCastNeed>()
                         // [BUG] Here must be WithDisabled, but it doesn't work for IBufferElementData
                         .WithNone<BufferSenseSightCastPending>()
                         .WithEntityAccess())
            {
                var receiverPosition = receiverTransform.ValueRO.Value.TransformPoint(receiverData.ValueRO.Offset);
                var needs = SystemAPI.GetBuffer<BufferSenseSightCastNeed>(receiver);

                for (var i = needs.Length - 1; i >= 0; i--)
                {
                    var source = needs[i].Source;
                    var sourceData = SystemAPI.GetComponentRO<ComponentSenseSightSource>(source);
                    var sourceTransform = SystemAPI.GetComponentRO<LocalToWorld>(source);
                    var sourcePosition = sourceTransform.ValueRO.Value.TransformPoint(sourceData.ValueRO.Offset);
                    var raycastData = new RaycastSenseSightCast
                    {
                        Receiver = receiver, ReceiverPosition = receiverPosition, SourcePosition = sourcePosition, Source = source,
                        NearClipRadiusSquared = receiverData.ValueRO.NearClipRadiusSquared,
                    };

                    commands.AppendToBuffer(receiver, new BufferSenseSightCastPending { Raycast = raycastData });
                    needs.RemoveAt(i);
                }

                commands.SetComponentEnabled<BufferSenseSightCastNeed>(receiver, false);
                commands.SetComponentEnabled<BufferSenseSightCastPending>(receiver, true);
            }

            commands.Playback(state.EntityManager);
        }
    }
}