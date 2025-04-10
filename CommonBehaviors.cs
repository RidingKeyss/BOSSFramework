// CommonBehaviors.cs
using System.Collections;
using UnityEngine;
using MelonLoader;
using Il2CppScheduleOne.NPCs;

namespace BOSSFramework
{
    public static class CommonBehaviors
    {
        public static IEnumerator MoveToRandom(NPC npc)
        {
            while (true)
            {
                Vector3 randomOffset = new Vector3(UnityEngine.Random.Range(-50f, 50f), 0f, UnityEngine.Random.Range(-50f, 50f));
                Vector3 target = npc.transform.position + randomOffset;
                yield return CommonActions.MoveTo(npc, target);
                yield return CommonActions.Wait(npc, 1.5f);
            }
        }

        public static IEnumerator FollowNearestNPC(NPC npc)
        {
            var allNpcs = GameObject.FindObjectsOfType<NPC>();
            NPC closest = null;
            float closestDist = float.MaxValue;

            foreach (var other in allNpcs)
            {
                if (other == npc) continue;
                float dist = Vector3.Distance(npc.transform.position, other.transform.position);
                if (dist < closestDist)
                {
                    closest = other;
                    closestDist = dist;
                }
            }

            if (closest != null)
            {
                while (true)
                {
                    yield return CommonActions.FollowNPC(npc, closest);
                }
            }
            else
            {
                MelonLogger.Msg("[BOSSFramework] No other NPCs found to follow.");
                yield return null;
            }
        }

        public static IEnumerator FollowNearestPlayer(NPC npc)
        {
            var allPlayers = GameObject.FindObjectsOfType<Il2CppScheduleOne.PlayerScripts.Player>();
            Il2CppScheduleOne.PlayerScripts.Player closest = null;
            float closestDist = float.MaxValue;

            foreach (var player in allPlayers)
            {
                float dist = Vector3.Distance(npc.transform.position, player.transform.position);
                if (dist < closestDist)
                {
                    closest = player;
                    closestDist = dist;
                }
            }

            if (closest != null)
            {
                yield return CommonActions.Say(npc, "Following you boss! I love Schedule One Modding discord!", 10f);
                while (true)
                {
                    yield return CommonActions.FollowPlayer(npc, closest);
                }
            }
            else
            {
                MelonLogger.Msg("[BOSSFramework] No players found to follow.");
                yield return null;
            }
        }

        public static void StopMovement(NPC npc)
        {
            if (npc.Movement.IsMoving)
            {
                npc.Movement.Stop();
            }
        }
    }
}
