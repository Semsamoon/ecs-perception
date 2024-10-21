using Unity.Entities;

namespace PerceptionECS
{
    [UpdateInGroup(typeof(GroupSenseUpdate))]
    public partial class GroupSenseUpdateLinecast : ComponentSystemGroup
    {
    }
}