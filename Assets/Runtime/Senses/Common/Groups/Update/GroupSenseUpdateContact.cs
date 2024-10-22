using Unity.Entities;

namespace ECSPerception
{
    [UpdateInGroup(typeof(GroupSenseUpdate)), UpdateAfter(typeof(GroupSenseUpdateLinecast))]
    public partial class GroupSenseUpdateContact : ComponentSystemGroup
    {
    }
}