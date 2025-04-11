//BOSSFramework - Il2Cpp/Il2CppBehaviorCloner.cs
//Helper for cloning behavior in shared core.
using BOSSCoreShared;

namespace BOSSIl2Cpp
{
    public class Il2CppBehaviorCloner : IBehaviorCloner
    {
        public IBehavior Clone(IBehavior template, string name)
        {
            var unityTemplate = Il2CppBehavior.Unwrap(template);
            if (unityTemplate == null)
            {
                MelonLoader.MelonLogger.Error("[BOSSFramework] IdleTemplate could not be unwrapped to Unity object.");
                return null!;
            }

            var clone = new Il2CppBehavior(UnityEngine.Object.Instantiate(unityTemplate));
            clone.Name = name;
            return clone;
        }
    }
}
