using Unity.Entities;

namespace PerceptionECS
{
    [UpdateInGroup(typeof(GroupSenseUpdate)), UpdateAfter(typeof(GroupSenseUpdateLinecast))]
    public partial class GroupSenseUpdateContact : ComponentSystemGroup
    {
    }
}