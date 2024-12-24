using ECSPerception.Groups;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace ECSPerception.Sight
{
    [BurstCompile]
    [UpdateInGroup(typeof(GroupSenses)), UpdateAfter(typeof(SystemSenseSightMultiCastNeed)), UpdateBefore(typeof(SystemSenseSightMultiCastExecute))]
    public partial struct SystemSenseSightCastExtraTwo : ISystem
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

            foreach (var (receiverData, castExtraTwo, receiverTransform, receiver) in SystemAPI
                         .Query<RefRO<ComponentSenseSightReceiver>, RefRO<ComponentSenseSightCastExtraTwo>, RefRO<LocalToWorld>>()
                         .WithAll<TagSenseSightMultiCast>()
                         .WithAll<BufferSenseSightNeedCast, BufferSenseSightCast>()
                         .WithEntityAccess())
            {
                var needCasts = SystemAPI.GetBuffer<BufferSenseSightNeedCast>(receiver);
                var receiverPosition = receiverTransform.ValueRO.Value.TransformPoint(receiverData.ValueRO.Offset);

                for (var i = needCasts.Length - 1; i >= 0; i--)
                {
                    var source = needCasts[i].Source;
                    var sourceTransform = SystemAPI.GetComponentRO<LocalToWorld>(source);
                    var sourceData = SystemAPI.GetComponentRO<ComponentSenseSightSource>(source);
                    var sourcePositionCenter = sourceTransform.ValueRO.Value.TransformPoint(sourceData.ValueRO.Offset);
                    var sourcePosition1 = sourcePositionCenter + castExtraTwo.ValueRO.Offset1;
                    var sourcePosition2 = sourcePositionCenter + castExtraTwo.ValueRO.Offset2;

                    commands.AppendToBuffer(receiver, new BufferSenseSightCast(
                        new RaycastSenseSightData(receiverPosition, receiver, sourcePositionCenter, source)));
                    commands.AppendToBuffer(receiver, new BufferSenseSightCast(
                        new RaycastSenseSightData(receiverPosition, receiver, sourcePosition1, source)));
                    commands.AppendToBuffer(receiver, new BufferSenseSightCast(
                        new RaycastSenseSightData(receiverPosition, receiver, sourcePosition2, source)));
                    needCasts.RemoveAt(i);
                }
            }

            commands.Playback(state.EntityManager);
        }
    }
}