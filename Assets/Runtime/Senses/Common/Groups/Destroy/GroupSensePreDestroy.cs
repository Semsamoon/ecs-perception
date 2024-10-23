using Unity.Entities;

namespace ECSPerception
{
    [UpdateInGroup(typeof(GroupSenseDestroy), OrderFirst = true)]
    public partial class GroupSensePreDestroy : ComponentSystemGroup
    {
    }
}