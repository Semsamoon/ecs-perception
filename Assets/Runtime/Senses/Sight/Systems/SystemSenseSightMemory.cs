using Unity.Burst;
using Unity.Entities;

namespace ECSPerception.Sight
{
    [BurstCompile]
    [UpdateInGroup(typeof(SightSenseSystemGroup), OrderFirst = true), UpdateAfter(typeof(SystemSenseSightInitialize))]
    public partial struct SystemSenseSightMemory : ISystem
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
            var deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (_, receiver) in SystemAPI
                         .Query<RefRO<ComponentSenseSightReceiver>>()
                         .WithAll<BufferSenseSightMemory>()
                         .WithEntityAccess())
            {
                var memories = SystemAPI.GetBuffer<BufferSenseSightMemory>(receiver);

                for (var i = 0; i < memories.Length; i++)
                {
                    var memory = memories[i];
                    memory.Timer -= deltaTime;

                    if (memory.Timer <= 0 || !SystemAPI.Exists(memory.Source))
                    {
                        memories.RemoveAtSwapBack(i);
                        i--;
                        continue;
                    }

                    memories[i] = memory;
                }
            }
        }
    }
}