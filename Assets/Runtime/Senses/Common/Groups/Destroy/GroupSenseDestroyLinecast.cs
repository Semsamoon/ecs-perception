using Unity.Entities;

namespace ECSPerception
{
    [UpdateInGroup(typeof(GroupSenseDestroy)), UpdateAfter(typeof(GroupSenseDestroyContact))]
    public partial class GroupSenseDestroyLinecast : ComponentSystemGroup
    {
    }
}