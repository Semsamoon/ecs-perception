using System;

namespace ECSPerception
{
    [Serializable]
    public struct SenseAffiliationFilter
    {
        public bool IsDetectEnemies;
        public bool IsDetectNeutrals;
        public bool IsDetectFriends;

        public static explicit operator ushort(SenseAffiliationFilter lhs)
        {
            return (ushort)((lhs.IsDetectEnemies ? 1 : 0) + (lhs.IsDetectNeutrals ? 2 : 0) + (lhs.IsDetectFriends ? 4 : 0));
        }
    }
}