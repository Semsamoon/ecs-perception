using Unity.Assertions;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;

namespace ECSPerception.Sight
{
    [BurstCompile]
    public struct UtilSenseSightRayCollector<T> : ICollector<T> where T : struct, IQueryResult
    {
        private readonly Entity _excludedEntity;
        private readonly float _minFraction;

        public T ClosestHit { get; private set; }
        public float MaxFraction { get; private set; }

        public bool EarlyOutOnFirstHit => false;
        public int NumHits => ClosestHit.Entity == Entity.Null ? 0 : 1;

        public UtilSenseSightRayCollector(Entity excludedEntity, float minFraction)
        {
            _excludedEntity = excludedEntity;
            _minFraction = minFraction;
            ClosestHit = default;
            MaxFraction = 1;
        }

        [BurstCompile]
        public bool AddHit(T hit)
        {
            Assert.IsTrue(hit.Fraction <= MaxFraction);

            if (hit.Entity == _excludedEntity || hit.Fraction < _minFraction)
            {
                return false;
            }

            MaxFraction = hit.Fraction;
            ClosestHit = hit;
            return true;
        }
    }
}