using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace ECSPerception.Sight
{
    [BurstCompile]
    [UpdateInGroup(typeof(SightSenseSystemGroup), OrderFirst = true)]
    public partial struct SystemSenseSightNeed : ISystem
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
            foreach (var (_, receiver) in SystemAPI
                         .Query<RefRO<ComponentSenseSightReceiver>>()
                         .WithAll<BufferSenseSightCastNeed>()
                         .WithEntityAccess())
            {
                var needs = SystemAPI.GetBuffer<BufferSenseSightCastNeed>(receiver);

                for (var i = needs.Length - 1; i >= 0; i--)
                {
                    var offset = SystemAPI.GetComponent<ComponentSenseSightSource>(needs[i].Source).Offset;
                    var sourcePosition = SystemAPI.GetComponent<LocalToWorld>(needs[i].Source).Value.TransformPoint(offset);

                    needs[i] = new BufferSenseSightCastNeed { Source = needs[i].Source, SourcePosition = sourcePosition };
                }
            }
        }
    }
}