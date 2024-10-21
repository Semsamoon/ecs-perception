using Unity.Entities;

namespace PerceptionECS
{
    [UpdateInGroup(typeof(SimulationSystemGroup)), UpdateAfter(typeof(GroupSenseDestroy))]
    public partial class GroupSenseCreate : ComponentSystemGroup
    {
    }
}