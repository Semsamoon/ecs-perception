using Unity.Entities;

namespace ECSPerception.Sight
{
    public struct BufferSenseSightActive : IBufferElementData
    {
        public Entity Source;

        public BufferSenseSightActive(Entity source)
        {
            Source = source;
        }
    }
}