using Unity.Entities;

namespace ECSPerception
{
    [UpdateInGroup(typeof(SimulationSystemGroup)), UpdateAfter(typeof(GroupSenseCreate))]
    public partial class GroupSenseUpdate : ComponentSystemGroup
    {
    }
}