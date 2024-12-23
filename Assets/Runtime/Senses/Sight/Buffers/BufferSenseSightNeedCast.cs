using Unity.Entities;

namespace ECSPerception.Sight
{
    public struct BufferSenseSightNeedCast : IBufferElementData
    {
        public Entity Source;

        public BufferSenseSightNeedCast(Entity source)
        {
            Source = source;
        }
    }
}