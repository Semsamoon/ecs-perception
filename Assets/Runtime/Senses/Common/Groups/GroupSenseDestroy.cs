using Unity.Entities;

namespace PerceptionECS
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class GroupSenseDestroy : ComponentSystemGroup
    {
    }
}