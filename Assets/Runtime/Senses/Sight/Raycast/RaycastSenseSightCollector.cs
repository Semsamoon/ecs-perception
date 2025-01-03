using Unity.Assertions;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;

namespace ECSPerception.Sight
{
    [BurstCompile]
    public struct RaycastSenseSightCollector<T> : ICollector<T> where T : struct, IQueryResult
    {
        private readonly Entity _excludedEntity;
        private readonly float _minFraction;

        private T _closestHit;
        public T ClosestHit => _closestHit;

        public bool EarlyOutOnFirstHit => false;
        public int NumHits => _closestHit.Entity == Entity.Null ? 0 : 1;

        public float MaxFraction { get; private set; }

        public RaycastSenseSightCollector(Entity excludedEntity, float minFraction)
        {
            _excludedEntity = excludedEntity;
            _closestHit = default;
            _minFraction = minFraction;
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
            _closestHit = hit;
            return true;
        }
    }
}