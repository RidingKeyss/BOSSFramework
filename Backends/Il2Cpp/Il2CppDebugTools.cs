// BOSSFramework - Backends/Il2Cpp/Il2CppDebugTools.cs
// IL2CPP-only debug tools for development/testing
using UnityEngine;
using MelonLoader;
using Il2CppScheduleOne.NPCs;
using Il2CppScheduleOne.PlayerScripts;
using BOSSFramework.Shared;
using BOSSFramework.Examples;

namespace BOSSFramework.Backends.Il2Cpp
{
    public static class Il2CppDebugTools
    {
        public static void RegisterDebugKeys()
        {
            MelonLogger.Msg("Debug Keys Registered.");
            MelonCoroutines.Start(DebugKeyWatcher());
        }

        private static System.Collections.IEnumerator DebugKeyWatcher()
        {
            while (true)
            {
                if (Input.GetKeyDown(KeyCode.P))
                {
                    TryApplyDebugBehavior();
                }
                yield return null;
            }
        }

        private static void TryApplyDebugBehavior()
        {
            var npcs = GameObject.FindObjectsOfType<NPC>();
            var player = GameObject.FindObjectOfType<Player>();
            if (player == null || npcs == null || npcs.Count == 0)
                return;

            var closestNpc = npcs
                .OrderBy(npc => Vector3.Distance(npc.transform.position, player.transform.position))
                .FirstOrDefault();

            if (closestNpc == null) return;

            var wrappedNpc = new Il2CppNPC(closestNpc);

            var tree = BTExampleTree.Create(wrappedNpc, new Il2CppPlayer(player));

            BehaviorRegistry.Apply(wrappedNpc, tree, "FollowExampleTree");

            MelonLogger.Msg($"[BOSSFramework] Applied test behavior to NPC: {wrappedNpc.Name}");
        }
    }
}
