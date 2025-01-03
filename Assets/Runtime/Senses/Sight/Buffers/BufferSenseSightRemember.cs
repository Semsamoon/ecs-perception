using Unity.Entities;

namespace ECSPerception.Sight
{
    public struct BufferSenseSightRemember : IBufferElementData
    {
        public Entity Source;
        public float Timer;
    }
}