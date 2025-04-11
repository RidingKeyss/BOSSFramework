// BOSSFramework - Mono/MonoDebugTools.cs
// Mono-only debug tools for development/testing

using UnityEngine;
using MelonLoader;
using ScheduleOne.NPCs;
using ScheduleOne.PlayerScripts;
using BOSSFramework.Examples;
using System.Collections;
using BOSSMono;

namespace BOSSFramework.Mono
{
    public static class MonoDebugTools
    {
        public static void RegisterDebugKeys()
        {
            MelonLogger.Msg("Debug Keys Registered.");
            MelonCoroutines.Start(DebugKeyWatcher());
        }

        private static IEnumerator DebugKeyWatcher()
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
            if (player == null || npcs == null || npcs.Length == 0)
                return;

            var closestNpc = npcs
                .OrderBy(npc => Vector3.Distance(npc.transform.position, player.transform.position))
                .FirstOrDefault();

            if (closestNpc == null) return;

            var wrappedNpc = new MonoNPC(closestNpc);
            var tree = BTExampleTree.Create(wrappedNpc, new MonoPlayer(player));

            BehaviorTree.BehaviorRegistry.Apply(wrappedNpc, tree, "FollowExampleTree");

            MelonLogger.Msg($"[BOSSFramework] Applied test behavior to NPC: {wrappedNpc.Name}");
        }
    }
}
