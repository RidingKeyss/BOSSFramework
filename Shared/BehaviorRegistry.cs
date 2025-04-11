// BOSSFramework - BehaviorRegistry.cs
// Manages direct application and cleanup of BehaviorTree instances on NPCs

using MelonLoader;

namespace BOSSFramework.Shared
{
    public static class BehaviorRegistry
    {
        private static readonly Dictionary<INPC, BehaviorTree.BehaviorTree> _activeTrees = new();
        private static readonly Dictionary<INPC, object> _activeCoroutines = new();
        private static readonly Dictionary<INPC, IBehavior> _activeBehaviours = new();

        public static void Apply(INPC npc, BehaviorTree.BehaviorTree tree, string name = "BOSS_Tree")
        {
            if (npc == null || tree == null)
            {
                MelonLogger.Warning("[BOSSFramework] Cannot apply behavior — NPC or tree is null.");
                return;
            }

            var template = npc.ActiveBehavior ?? BOSSUtils.IdleTemplate;
            if (template == null)
            {
                MelonLogger.Error($"[BOSSFramework] Cannot apply behavior — no valid behavior template.");
                return;
            }

            PauseExistingBehaviours(npc);

            if (template == null)
            {
                MelonLogger.Warning("[BOSSFramework] Cannot apply behavior — IdleTemplate is null.");
                return;
            }

            MelonLogger.Msg($"[BOSSFramework] Attempting to clone IdleTemplate of type: {template.GetType()}");


            var unityTemplate = Backends.Il2Cpp.Il2CppBehavior.Unwrap(template);
            if (unityTemplate == null)
            {
                MelonLogger.Error("[BOSSFramework] IdleTemplate could not be unwrapped to Unity object.");
                return;
            }
            var cloned = new Backends.Il2Cpp.Il2CppBehavior(UnityEngine.Object.Instantiate(unityTemplate));
            cloned.Name = name;
            npc.AddEnabledBehavior(cloned);
            npc.ActiveBehavior = cloned;

            var coroutine = MelonCoroutines.Start(tree.Run());
            _activeTrees[npc] = tree;
            _activeCoroutines[npc] = coroutine;
            _activeBehaviours[npc] = cloned;

            MelonLogger.Msg($"[BOSSFramework] Applied behavior '{name}' to NPC: {npc.Name}");
        }

        public static void Remove(INPC npc)
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
                npc.RemoveEnabledBehavior(behaviour);
                _activeBehaviours.Remove(npc);
            }

            ResumeExistingBehaviours(npc);
            MelonLogger.Msg($"[BOSSFramework] Removed behavior from NPC: {npc.Name}");
        }

        public static void RemoveAll()
        {
            var npcs = _activeTrees.Keys.ToList();
            foreach (var npc in npcs)
                Remove(npc);

            MelonLogger.Msg("[BOSSFramework] Removed all active behavior trees.");
        }

        public static void PauseExistingBehaviours(INPC npc)
        {
            var existing = npc.EnabledBehaviors;
            foreach (var behaviour in existing)
            {
                if (behaviour.Active)
                {
                    behaviour.Active = false;
                    behaviour.BehaviourUpdate();
                    if (npc.LocalConnection != null)
                        behaviour.EndNetworked(npc.LocalConnection);

                    MelonLogger.Msg($"[BOSSFramework] Paused behaviour: {behaviour.Name}");
                }
            }
        }

        public static void ResumeExistingBehaviours(INPC npc)
        {
            var existing = npc.EnabledBehaviors;
            foreach (var behaviour in existing)
            {
                if (!behaviour.Active)
                {
                    behaviour.Active = true;
                    if (npc.LocalConnection != null)
                    {
                        behaviour.EnableNetworked(npc.LocalConnection);
                        behaviour.BeginNetworked(npc.LocalConnection);
                    }
                    MelonLogger.Msg($"[BOSSFramework] Resumed behaviour: {behaviour.Name}");
                }
            }

            if (existing.Count > 0)
            {
                npc.ActiveBehavior = existing[0];
                MelonLogger.Msg($"[BOSSFramework] Set activeBehaviour to: {existing[0]?.Name}");
            }
        }
    }
}
