// BehaviorRegistry.cs
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MelonLoader;
using Il2CppScheduleOne.NPCs;
using Il2CppScheduleOne.NPCs.Behaviour;

namespace BOSSFramework
{
    public static class BehaviorRegistry
    {
        public struct BOSSBehavior
        {
            public Func<NPC, IEnumerator> Start;
            public Action<NPC> Stop;

            public BOSSBehavior(Func<NPC, IEnumerator> start, Action<NPC> stop = null)
            {
                Start = start;
                Stop = stop;
            }
        }

        private static readonly Dictionary<string, BOSSBehavior> _behaviors = new();
        private static readonly Dictionary<NPC, object> _activeCoroutines = new();
        private static readonly Dictionary<NPC, Il2CppScheduleOne.NPCs.Behaviour.Behaviour> _activeBehaviours = new();

        public static void Register(string id, Func<NPC, IEnumerator> start, Action<NPC> stop = null)
        {
            if (_behaviors.ContainsKey(id))
            {
                MelonLogger.Warning($"[BOSSFramework] Behavior '{id}' is already registered.");
                return;
            }
            _behaviors.Add(id, new BOSSBehavior(start, stop));
            MelonLogger.Msg($"[BOSSFramework] Registered behavior: {id}");
        }

        public static void Start(NPC npc, string id)
        {
            if (!_behaviors.TryGetValue(id, out var behavior))
            {
                MelonLogger.Error($"[BOSSFramework] Behavior '{id}' not found.");
                return;
            }

            // Clone and spawn a base behaviour to use as a stub
            var original = npc.behaviour.activeBehaviour;
            var customBehaviour = UnityEngine.Object.Instantiate(original);
            customBehaviour.name = $"BOSS_{id}";

            npc.behaviour.AddEnabledBehaviour(customBehaviour);
            npc.behaviour.activeBehaviour = customBehaviour;

            // Start and track coroutine
            IEnumerator routine = behavior.Start.Invoke(npc);
            var coroutineToken = MelonCoroutines.Start(routine);
            _activeCoroutines[npc] = coroutineToken;
            _activeBehaviours[npc] = customBehaviour;
        }

        public static void Stop(NPC npc)
        {
            if (_activeCoroutines.TryGetValue(npc, out var coroutineToken))
            {
                if (coroutineToken != null)
                {
                    MelonCoroutines.Stop(coroutineToken);
                }
                _activeCoroutines.Remove(npc);
            }


            if (_activeBehaviours.TryGetValue(npc, out var behaviour))
            {
                var behaviorId = behaviour.name.Replace("BOSS_", "");
                if (_behaviors.TryGetValue(behaviorId, out var bossBehavior) && bossBehavior.Stop != null)
                {
                    bossBehavior.Stop.Invoke(npc);
                }
                npc.behaviour.RemoveEnabledBehaviour(behaviour);
                ResumeExistingBehaviours(npc);
                _activeBehaviours.Remove(npc);
            }
        }

        public static void StopAll()
        {
            var npcs = _activeBehaviours.Keys.ToArray(); // ToArray to avoid modifying during enumeration

            foreach (var npc in npcs)
            {
               MelonLogger.Msg($"[BOSSFramework] Stopping behavior on NPC: {npc.name}");
                Stop(npc);
            }

            MelonLogger.Msg("[BOSSFramework] Stopped all active behaviors.");
        }

        public static void PauseExistingBehaviours(NPC npc)
        {
            var existing = npc.behaviour.enabledBehaviours;
            for (int i = 0; i < existing.Count; i++)
            {
                if (existing[i].Active == true)
                {
                    existing[i].Active = false;
                    existing[i].BehaviourUpdate();
                    existing[i].End_Networked(npc.LocalConnection);
                    MelonLogger.Msg($"[BOSSFramework] Paused behaviour: {existing[i]?.name}");
                }
            }
        }
        public static void ResumeExistingBehaviours(NPC npc)
        {
            var existing = npc.behaviour.enabledBehaviours;
            for (int i = 0; i < existing.Count; i++)
            {
                if (existing[i].Active == false)
                {
                    existing[i].Active = true;
                    existing[i].BehaviourUpdate();
                    existing[i].Enable_Networked(npc.LocalConnection);
                    existing[i].Begin_Networked(npc.LocalConnection);
                    MelonLogger.Msg($"[BOSSFramework] Resumed behaviour: {existing[i]?.name}");
                }

                if (existing.Count > 0)
                {
                    npc.behaviour.activeBehaviour = existing[0];
                    MelonLogger.Msg($"[BOSSFramework] Set activeBehaviour to: {existing[0]?.name}");
                }
            }
        }

        public static IEnumerable<string> List() => _behaviors.Keys;

        public static bool TryGetActiveBehaviour(NPC npc, out Il2CppScheduleOne.NPCs.Behaviour.Behaviour behaviour)
        {
            return _activeBehaviours.TryGetValue(npc, out behaviour);
        }
    }
}
