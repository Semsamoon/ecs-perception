using Unity.Entities;

namespace PerceptionECS
{
    [UpdateInGroup(typeof(SimulationSystemGroup)), UpdateAfter(typeof(GroupSenseCreate))]
    public partial class GroupSenseUpdate : ComponentSystemGroup
    {
    }
}