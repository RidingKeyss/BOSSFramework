// BOSSFramework - Mono/MonoIdleTemplateProvider.cs
// Mono implementation of IIdleTemplateProvider
using BOSSCoreShared;
using ScheduleOne.NPCs;
using UnityEngine;
using System.Reflection;

namespace BOSSMono
{
    public class MonoIdleTemplateProvider : IIdleTemplateProvider
    {
        public IBehavior? GetIdleTemplate()
        {
            var npcs = GameObject.FindObjectsOfType<NPC>();
            foreach (var npc in npcs)
            {
                var stack = MonoReflectionUtils.GetBehaviourStack(npc.behaviour, "behaviourStack");
                if (stack == null) continue;

                foreach (var b in stack)
                {
                    if (b != null && b.GetType().Name == "StationaryBehaviour")
                    {
                        var clone = UnityEngine.Object.Instantiate(b) as ScheduleOne.NPCs.Behaviour.Behaviour;
                        if (clone == null)
                        {
                            MelonLoader.MelonLogger.Warning("[BOSSFramework] [Mono] Failed to cast cloned behavior to ScheduleOne.Behaviour.");
                            continue;
                        }
                        MelonLoader.MelonLogger.Msg("[BOSSFramework] [Mono] Cached StationaryBehaviour as IdleTemplate.");
                        return new MonoBehavior(clone);
                    }
                }
            }

            MelonLoader.MelonLogger.Warning("[BOSSFramework] [Mono] Could not find any StationaryBehaviour to use as a fallback template.");
            return null;
        }
    }
}
