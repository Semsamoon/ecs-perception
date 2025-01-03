using Unity.Entities;

namespace ECSPerception
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class SensesSystemGroup : ComponentSystemGroup
    {
    }
}