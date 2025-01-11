using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace ECSPerception.Sight
{
    [BurstCompile]
    [UpdateInGroup(typeof(SightSenseSystemGroup), OrderFirst = true), UpdateAfter(typeof(SystemSenseSightInitialize))]
    public partial struct SystemSenseSightPossible : ISystem
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
                         .WithAll<BufferSenseSightPossible>()
                         .WithEntityAccess())
            {
                var possibles = SystemAPI.GetBuffer<BufferSenseSightPossible>(receiver);

                for (var i = 0; i < possibles.Length; i++)
                {
                    var source = possibles[i].Source;

                    if (!SystemAPI.Exists(source))
                    {
                        possibles.RemoveAtSwapBack(i);
                        i--;
                        continue;
                    }

                    var offset = SystemAPI.GetComponent<ComponentSenseSightSource>(source).Offset;
                    var sourceTransform = SystemAPI.GetComponent<LocalToWorld>(source);

                    possibles[i] = new BufferSenseSightPossible
                    {
                        Source = source,
                        SourcePosition = sourceTransform.Value.TransformPoint(offset),
                    };
                }
            }
        }
    }
}