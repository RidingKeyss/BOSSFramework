// BOSSFramework - Mono/MonoRelfectionutils.cs
// Utils for easy behaviour stack reflection
using System.Reflection;
using ScheduleOne.NPCs.Behaviour;

namespace BOSSMono
{
    public static class MonoReflectionUtils
    {
        public static List<Behaviour>? GetBehaviourStack(object behaviourContainer)
        {
            var field = behaviourContainer.GetType().GetField("behaviourStack", BindingFlags.Instance | BindingFlags.NonPublic);
            return field?.GetValue(behaviourContainer) as List<Behaviour>;
        }
    }
}