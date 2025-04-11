// BOSSFramework - MonoBehaviorCloner.cs
// Mono implementation of IBehaviorCloner

using BOSSCoreShared;

namespace BOSSMono
{
    public class MonoBehaviorCloner : IBehaviorCloner
    {
        public IBehavior Clone(IBehavior template, string name)
        {
            var unityTemplate = MonoBehavior.Unwrap(template);
            if (unityTemplate == null)
            {
                MelonLoader.MelonLogger.Error("[BOSSFramework] [Mono] IdleTemplate could not be unwrapped to a valid behavior.");
                return null!;
            }

            var clone = UnityEngine.Object.Instantiate(unityTemplate);
            clone.name = name;
            return new MonoBehavior(clone);
        }
    }
}
