using Unity.Burst;
using Unity.Collections;

namespace ECSPerception.Sight
{
    [BurstCompile]
    public struct UtilSenseSightRayEnumerator
    {
        private readonly NativeArray<BufferSenseSightExecute> _executes;
        private readonly NativeArray<UtilSenseSightRayMeta> _metas;
        private readonly NativeArray<bool> _results;

        private int _index;

        public UtilSenseSightRayEnumerator(NativeArray<BufferSenseSightExecute> executes,
            NativeArray<UtilSenseSightRayMeta> metas, NativeArray<bool> results)
        {
            _executes = executes;
            _metas = metas;
            _results = results;
            _index = 0;
        }

        [BurstCompile]
        public bool MoveNext()
        {
            return _index < _executes.Length;
        }

        [BurstCompile]
        public (BufferSenseSightExecute executed, UtilSenseSightRayMeta meta, bool result) GetCurrent()
        {
            var executed = _executes[_index];
            var meta = _metas[_index];
            var result = _results[_index];
            _index++;

            while (_index < _executes.Length && _executes[_index].Source == executed.Source)
            {
                if (!result && _results[_index])
                {
                    executed = _executes[_index];
                    meta = _metas[_index];
                    result = true;
                }

                _index++;
            }

            return (executed, meta, result);
        }
    }
}