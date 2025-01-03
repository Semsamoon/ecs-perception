using ECSPerception.Groups;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace ECSPerception.Sight
{
    [BurstCompile]
    [UpdateInGroup(typeof(GroupSenses))]
    public partial struct SystemSenseSightCastCone : ISystem
    {
        private EntityQuery _queryCastsNeedOrPending;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _queryCastsNeedOrPending = SystemAPI.QueryBuilder()
                .WithAny<BufferSenseSightCastNeed, BufferSenseSightCastPending>()
                .Build();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (!_queryCastsNeedOrPending.IsEmpty)
            {
                return;
            }

            var commands = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (receiverData, receiverTransform, receiver) in SystemAPI
                         .Query<RefRO<ComponentSenseSightReceiver>, RefRO<LocalToWorld>>()
                         .WithAll<BufferSenseSightActive>()
                         .WithPresent<BufferSenseSightCastNeed>()
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
                            commands.AppendToBuffer(receiver,
                                new BufferSenseSightRemember { Source = source, Timer = receiverData.ValueRO.RememberTime });
                        }

                        continue;
                    }

                    commands.AppendToBuffer(receiver, new BufferSenseSightCastNeed { Source = source });
                }

                commands.SetComponentEnabled<BufferSenseSightCastNeed>(receiver, true);
            }

            commands.Playback(state.EntityManager);
        }
    }
}