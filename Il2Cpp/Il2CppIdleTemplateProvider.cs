// BOSSFramework - Il2CppIdleTemplateProvider.cs
// IL2CPP implementation of IIdleTemplateProvider
using Il2CppScheduleOne.NPCs;
using BOSSCoreShared;
using MelonLoader;

namespace BOSSIl2Cpp
{
    public class Il2CppIdleTemplateProvider : IIdleTemplateProvider
    {
        public IBehavior GetIdleTemplate()
        {
            var npcs = UnityEngine.Object.FindObjectsOfType<NPC>();
            foreach (var npc in npcs)
            {
                foreach (var b in npc.behaviour.behaviourStack)
                {
                    if (b != null && b.GetIl2CppType().Name == "StationaryBehaviour")
                    {
                        var clone = UnityEngine.Object.Instantiate(b);
                        MelonLogger.Msg("[BOSSFramework] Cached StationaryBehaviour as IdleTemplate.");
                        MelonLogger.Msg($"[BOSSFramework] Clone success? {clone != null}");
                        MelonLogger.Msg($"[BOSSFramework] Clone type: {clone?.GetType().FullName}");
                        return new Il2CppBehavior(clone);
                    }
                }
            }

            MelonLogger.Warning("[BOSSFramework] Could not find a StationaryBehaviour to use as IdleTemplate.");
            return null;
        }
    }
}
