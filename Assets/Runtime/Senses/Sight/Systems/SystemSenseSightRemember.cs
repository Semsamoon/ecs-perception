using ECSPerception.Groups;
using Unity.Burst;
using Unity.Entities;

namespace ECSPerception.Sight
{
    [BurstCompile]
    [UpdateInGroup(typeof(GroupSenses), OrderFirst = true)]
    public partial struct SystemSenseSightRemember : ISystem
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
                         .WithAll<BufferSenseSightRemember>()
                         .WithEntityAccess())
            {
                var remembers = SystemAPI.GetBuffer<BufferSenseSightRemember>(receiver);

                for (var i = remembers.Length - 1; i >= 0; i--)
                {
                    var remember = remembers[i];
                    remember.Timer -= deltaTime;

                    if (remember.Timer > 0)
                    {
                        remembers[i] = remember;
                        continue;
                    }

                    remembers.RemoveAt(i);
                }
            }
        }
    }
}