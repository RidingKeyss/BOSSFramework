// BOSSFramework - BehaviorRegistry.cs
// Manages direct application and cleanup of BehaviorTree instances on NPCs

using MelonLoader;
using Il2CppScheduleOne.NPCs;

namespace BOSSFramework
{
    public static class BehaviorRegistry
    {
        private static readonly Dictionary<NPC, BehaviorTree.BehaviorTree> _activeTrees = new();
        private static readonly Dictionary<NPC, object> _activeCoroutines = new();
        private static readonly Dictionary<NPC, Il2CppScheduleOne.NPCs.Behaviour.Behaviour> _activeBehaviours = new();

        public static void Apply(NPC npc, BehaviorTree.BehaviorTree tree, string name = "BOSS_Tree")
        {
            if (npc == null || tree == null)
            {
                MelonLogger.Warning("[BOSSFramework] Cannot apply behavior — NPC or tree is null.");
                return;
            }

            var template = npc.behaviour.activeBehaviour ?? BOSSUtils.IdleTemplate;
            if (template == null)
            {
                MelonLogger.Error($"[BOSSFramework] Cannot apply behavior — no valid behavior template.");
                return;
            }

            PauseExistingBehaviours(npc);

            var cloned = UnityEngine.Object.Instantiate(template);
            cloned.name = name;
            npc.behaviour.AddEnabledBehaviour(cloned);
            npc.behaviour.activeBehaviour = cloned;

            var coroutine = MelonCoroutines.Start(tree.Run());
            _activeTrees[npc] = tree;
            _activeCoroutines[npc] = coroutine;
            _activeBehaviours[npc] = cloned;

            MelonLogger.Msg($"[BOSSFramework] Applied behavior '{name}' to NPC: {npc.name}");
        }

        public static void Remove(NPC npc)
        {
            if (npc == null) return;

            if (_activeCoroutines.TryGetValue(npc, out var coroutine))
            {
                if (coroutine != null)
                    MelonCoroutines.Stop(coroutine);
                _activeCoroutines.Remove(npc);
            }

            if (_activeTrees.ContainsKey(npc))
            {
                _activeTrees[npc].Stop();
                _activeTrees.Remove(npc);
            }

            if (_activeBehaviours.TryGetValue(npc, out var behaviour))
            {
                npc.behaviour.RemoveEnabledBehaviour(behaviour);
                _activeBehaviours.Remove(npc);
            }

            ResumeExistingBehaviours(npc);
            MelonLogger.Msg($"[BOSSFramework] Removed behavior from NPC: {npc.name}");
        }

        public static void RemoveAll()
        {
            var npcs = _activeTrees.Keys.ToList();
            foreach (var npc in npcs)
                Remove(npc);

            MelonLogger.Msg("[BOSSFramework] Removed all active behavior trees.");
        }

        public static void PauseExistingBehaviours(NPC npc)
        {
            var existing = npc.behaviour.enabledBehaviours;
            foreach (var behaviour in existing)
            {
                if (behaviour.Active)
                {
                    behaviour.Active = false;
                    behaviour.BehaviourUpdate();
                    if (npc.LocalConnection != null)
                        behaviour.End_Networked(npc.LocalConnection);

                    MelonLogger.Msg($"[BOSSFramework] Paused behaviour: {behaviour.name}");
                }
            }
        }

        public static void ResumeExistingBehaviours(NPC npc)
        {
            var existing = npc.behaviour.enabledBehaviours;
            foreach (var behaviour in existing)
            {
                if (!behaviour.Active)
                {
                    behaviour.Active = true;
                    behaviour.BehaviourUpdate();
                    if (npc.LocalConnection != null)
                    {
                        behaviour.Enable_Networked(npc.LocalConnection);
                        behaviour.Begin_Networked(npc.LocalConnection);
                    }
                    MelonLogger.Msg($"[BOSSFramework] Resumed behaviour: {behaviour.name}");
                }
            }

            if (existing.Count > 0)
            {
                npc.behaviour.activeBehaviour = existing[0];
                MelonLogger.Msg($"[BOSSFramework] Set activeBehaviour to: {existing[0]?.name}");
            }
        }
    }
}
