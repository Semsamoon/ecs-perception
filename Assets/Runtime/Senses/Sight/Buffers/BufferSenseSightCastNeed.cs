using Unity.Entities;

namespace ECSPerception.Sight
{
    public struct BufferSenseSightCastNeed : IBufferElementData, IEnableableComponent
    {
        public Entity Source;

        public BufferSenseSightCastNeed(Entity source)
        {
            Source = source;
        }
    }
}