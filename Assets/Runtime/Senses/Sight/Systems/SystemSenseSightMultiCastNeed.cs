using ECSPerception.Groups;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace ECSPerception.Sight
{
    [BurstCompile]
    [UpdateInGroup(typeof(GroupSenses)), UpdateBefore(typeof(SystemSenseSightMultiCastExecute))]
    public partial struct SystemSenseSightMultiCastNeed : ISystem
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
                         .WithAll<TagSenseSightMultiCast>()
                         .WithAll<BufferSenseSightActive, BufferSenseSightNeedCast, BufferSenseSightRemember>()
                         .WithEntityAccess())
            {
                var actives = SystemAPI.GetBuffer<BufferSenseSightActive>(receiver);

                foreach (var (sourceData, sourceTransform, source) in SystemAPI
                             .Query<RefRO<ComponentSenseSightSource>, RefRO<LocalToWorld>>()
                             .WithEntityAccess())
                {
                    if (!ConecastSenseSight.IsInside(receiverData, receiverTransform, sourceData, sourceTransform, actives.Has(source)))
                    {
                        if (actives.Remove(source) && receiverData.ValueRO.RememberTime > 0)
                        {
                            commands.AppendToBuffer(receiver, new BufferSenseSightRemember(source, receiverData.ValueRO.RememberTime));
                        }

                        continue;
                    }

                    commands.AppendToBuffer(receiver, new BufferSenseSightNeedCast(source));
                }
            }

            commands.Playback(state.EntityManager);
        }
    }
}