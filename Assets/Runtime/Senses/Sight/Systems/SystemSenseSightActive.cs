using ECSPerception.Groups;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace ECSPerception.Sight
{
    [BurstCompile]
    [UpdateInGroup(typeof(GroupSenses), OrderLast = true), UpdateAfter(typeof(SystemSenseSightCastExecute))]
    public partial struct SystemSenseSightActive : ISystem
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
                         .WithAll<BufferSenseSightActive>()
                         .WithEntityAccess())
            {
                var actives = SystemAPI.GetBuffer<BufferSenseSightActive>(receiver);

                for (var i = actives.Length - 1; i >= 0; i--)
                {
                    var sourcePosition = SystemAPI.GetComponent<LocalToWorld>(actives[i].Source).Position;
                    actives[i] = new BufferSenseSightActive { Source = actives[i].Source, SourcePosition = sourcePosition };
                }
            }
        }
    }
}