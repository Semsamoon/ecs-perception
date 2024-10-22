using Unity.Entities;

namespace ECSPerception
{
    [UpdateInGroup(typeof(GroupSenseUpdate))]
    public partial class GroupSenseUpdateLinecast : ComponentSystemGroup
    {
    }
}