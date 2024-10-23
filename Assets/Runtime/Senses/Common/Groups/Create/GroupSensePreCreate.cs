using Unity.Entities;

namespace ECSPerception
{
    [UpdateInGroup(typeof(GroupSenseCreate), OrderFirst = true)]
    public partial class GroupSensePreCreate : ComponentSystemGroup
    {
    }
}