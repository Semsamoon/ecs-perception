using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace ECSPerception.Sight
{
    [BurstCompile]
    [UpdateInGroup(typeof(SightSenseSystemGroup), OrderLast = true), UpdateAfter(typeof(SystemSenseSightExecute))]
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

                for (var i = 0; i < actives.Length; i++)
                {
                    var source = actives[i].Source;

                    if (!SystemAPI.Exists(source))
                    {
                        actives.RemoveAtSwapBack(i);
                        i--;
                        continue;
                    }

                    var offset = SystemAPI.GetComponent<ComponentSenseSightSource>(source).Offset;
                    var sourceTransform = SystemAPI.GetComponent<LocalToWorld>(source);

                    actives[i] = new BufferSenseSightActive
                    {
                        Source = source,
                        SourcePosition = sourceTransform.Value.TransformPoint(offset),
                    };
                }
            }
        }
    }
}