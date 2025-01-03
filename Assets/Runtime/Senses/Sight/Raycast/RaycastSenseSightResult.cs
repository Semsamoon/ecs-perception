using Unity.Burst;
using Unity.Collections;

namespace ECSPerception.Sight
{
    [BurstCompile]
    public struct RaycastSenseSightResult
    {
        private readonly NativeArray<RaycastSenseSightCast> _raycasts;
        private readonly NativeArray<RaycastSenseSightCastMeta> _raycastsMeta;
        private readonly NativeArray<bool> _results;
        private readonly int _raycastsAmount;

        private int _index;

        public RaycastSenseSightResult(
            NativeArray<RaycastSenseSightCast> raycasts, NativeArray<RaycastSenseSightCastMeta> raycastsMeta,
            NativeArray<bool> results, int raycastsAmount)
        {
            _raycasts = raycasts;
            _raycastsMeta = raycastsMeta;
            _results = results;
            _raycastsAmount = raycastsAmount;
            _index = 0;
        }

        [BurstCompile]
        public bool MoveNext()
        {
            return _index < _raycastsAmount;
        }

        [BurstCompile]
        public (RaycastSenseSightCast raycast, RaycastSenseSightCastMeta raycastMeta, bool result) GetCurrent()
        {
            var raycast = _raycasts[_index];
            var raycastMeta = _raycastsMeta[_index];
            var result = _results[_index];
            _index++;

            while (_index < _raycastsAmount && _raycasts[_index].Source == raycast.Source)
            {
                if (!result && _results[_index])
                {
                    raycast = _raycasts[_index];
                    raycastMeta = _raycastsMeta[_index];
                    result = true;
                }

                _index++;
            }

            return (raycast, raycastMeta, result);
        }
    }
}