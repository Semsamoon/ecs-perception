using Unity.Entities;

namespace ECSPerception.Sight
{
    public struct BufferSenseSightRemember : IBufferElementData
    {
        public Entity Source;
        public float Timer;

        public BufferSenseSightRemember(Entity source, float timer)
        {
            Source = source;
            Timer = timer;
        }
    }
}