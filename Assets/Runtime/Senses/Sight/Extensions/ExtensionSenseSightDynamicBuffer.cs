using Unity.Burst;
using Unity.Entities;

namespace ECSPerception.Sight
{
    [BurstCompile]
    public static class ExtensionSenseSightDynamicBuffer
    {
        [BurstCompile]
        public static bool Take(this ref DynamicBuffer<BufferSenseSightActive> buffer, in Entity entity, out BufferSenseSightActive active)
        {
            active = default;

            for (var i = 0; i < buffer.Length; i++)
            {
                if (buffer[i].Source == entity)
                {
                    active = buffer[i];
                    buffer.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        [BurstCompile]
        public static bool Has(this in DynamicBuffer<BufferSenseSightActive> buffer, in Entity entity)
        {
            for (var i = 0; i < buffer.Length; i++)
            {
                if (buffer[i].Source == entity)
                {
                    return true;
                }
            }

            return false;
        }

        [BurstCompile]
        public static bool Remove(this ref DynamicBuffer<BufferSenseSightRemember> buffer, in Entity entity)
        {
            for (var i = 0; i < buffer.Length; i++)
            {
                if (buffer[i].Source == entity)
                {
                    buffer.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        [BurstCompile]
        public static bool Has(this in DynamicBuffer<BufferSenseSightRemember> buffer, in Entity entity)
        {
            for (var i = 0; i < buffer.Length; i++)
            {
                if (buffer[i].Source == entity)
                {
                    return true;
                }
            }

            return false;
        }
    }
}