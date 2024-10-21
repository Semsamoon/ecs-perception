using Unity.Entities;

namespace PerceptionECS
{
    [UpdateInGroup(typeof(GroupSenseCreate))]
    public partial class GroupSenseCreateLinecast : ComponentSystemGroup
    {
    }
}