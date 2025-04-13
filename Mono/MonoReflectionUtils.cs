// BOSSFramework - Mono/MonoRelfectionutils.cs
// Utils for easy behaviour stack reflection
using System.Reflection;
using ScheduleOne.NPCs.Behaviour;

namespace BOSSMono
{
    public static class MonoReflectionUtils
    {
        public static List<Behaviour>? GetBehaviourStack(object behaviourContainer, string behaviourStackName)
        {
            var field = behaviourContainer.GetType().GetField(behaviourStackName, BindingFlags.Instance | BindingFlags.NonPublic);
            return field?.GetValue(behaviourContainer) as List<Behaviour>;
        }
    }
}