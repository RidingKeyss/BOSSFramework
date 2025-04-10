// CommonActions.cs
using System.Collections;
using UnityEngine;
using MelonLoader;
using Il2CppScheduleOne.NPCs;
using Il2CppScheduleOne.PlayerScripts;


namespace BOSSFramework
{
    public static class CommonActions
    {
        public static IEnumerator MoveTo(NPC npc, Vector3 destination)
        {
            if (npc.Movement.CanMove())
            {
                if (npc.Movement.GetClosestReachablePoint(destination, out Vector3 reachable))
                {
                    npc.Movement.SetDestination(reachable);
                    npc.Movement.ResumeMovement();
                    MelonLogger.Msg($"[BOSSFramework] {npc.name} moving to {reachable}");

                    while (npc.Movement.IsMoving)
                    {
                        yield return null;
                    }
                }
                else
                {
                    MelonLogger.Msg($"[BOSSFramework] {npc.name} could not find path to destination.");
                }
            }
            else
            {
                if (npc.isInBuilding)
                {
                    npc.ExitBuilding();
                    npc.Movement.PauseMovement();
                }
                if (npc.IsInVehicle)
                {
                    npc.ExitVehicle();
                    npc.Movement.PauseMovement();
                }
                else
                {
                    MelonLogger.Msg($"[BOSSFramework] {npc.name} can not move.");
                }
            }
        }

        public static IEnumerator Say(NPC npc, string dialogue, float seconds)
        {
            MelonLogger.Msg($"[BOSSFramework] {npc.name} saying {dialogue} for {seconds} seconds");
            var renderer = BOSSUtils.GetWorldspaceDialogueRenderer(npc);
            renderer.ShowText(dialogue, seconds);
            npc.PlayVO(Il2CppScheduleOne.VoiceOver.EVOLineType.Acknowledge);
            yield return null;
        }

        public static IEnumerator Wait(NPC npc, float seconds)
        {
            MelonLogger.Msg($"[BOSSFramework] {npc.name} waiting for {seconds} seconds");
            yield return new WaitForSeconds(seconds);
        }

        public static IEnumerator FollowNPC(NPC npc, NPC target, float followDistance = 3f)
        {
            MelonLogger.Msg($"[BOSSFramework] {npc.name} is following {target.name}");

            float distance = Vector3.Distance(npc.transform.position, target.transform.position);

            if (distance > followDistance)
            {
                npc.Movement.SetDestination(target.transform.position);
                npc.Movement.ResumeMovement();
            }
            else if (npc.Movement.IsMoving)
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);
        }
        public static IEnumerator FollowPlayer(NPC npc, Player target, float followDistance = 3f)
        {
            MelonLogger.Msg($"[BOSSFramework] {npc.name} is following {target.name}");

            float distance = Vector3.Distance(npc.transform.position, target.transform.position);

            if (distance > followDistance)
            {
                npc.Movement.SetDestination(target.transform.position);
                npc.Movement.ResumeMovement();
            }
            else if (npc.Movement.IsMoving)
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
}
