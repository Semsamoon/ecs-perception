using Unity.Burst;
using Unity.Entities;

namespace ECSPerception.Sight
{
    [BurstCompile]
    public static class ExtensionSenseSightDynamicBuffer
    {
        [BurstCompile]
        public static bool Remove(this ref DynamicBuffer<BufferSenseSightActive> buffer, in Entity entity)
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
        public static bool Has(this ref DynamicBuffer<BufferSenseSightActive> buffer, in Entity entity)
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
        public static bool Has(this ref DynamicBuffer<BufferSenseSightRemember> buffer, in Entity entity)
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