// BOSSFramework - BTTasks.cs
// Implements reusable TaskNodes for core NPC actions

using System.Collections;
using UnityEngine;
using MelonLoader;
using Il2CppScheduleOne.NPCs;
using Il2CppScheduleOne.PlayerScripts;

namespace BOSSFramework.BehaviorTree.Tasks
{
    public class MoveToTask : TaskNode
    {
        public override IEnumerator Execute(BOSSBlackboard blackboard, Action<NodeState> callback)
        {
            var npc = blackboard.Get<NPC>("Self");
            var target = blackboard.Get<Player>("Player");
            if (npc == null || target == null)
            {
                callback(NodeState.Failure);
                yield break;
            }

            if (npc.Movement.CanMove())
            {
                if (npc.Movement.GetClosestReachablePoint(target.transform.position, out Vector3 reachable))
                {
                    npc.Movement.SetDestination(reachable);
                    npc.Movement.ResumeMovement();
                    MelonLogger.Msg($"[BOSSFramework] {npc.name} moving to {reachable}");

                    while (npc.Movement.IsMoving)
                    {
                        callback(NodeState.Running);
                    }

                    callback(NodeState.Success);
                    yield break;
                }
                else
                {
                    MelonLogger.Msg($"[BOSSFramework] {npc.name} could not find path to destination.");
                }
            }
            else
            {
                if (npc.isInBuilding) npc.ExitBuilding();
                if (npc.IsInVehicle) npc.ExitVehicle();
                npc.Movement.PauseMovement();
                MelonLogger.Msg($"[BOSSFramework] {npc.name} cannot move.");
            }

            callback(NodeState.Failure);
        }
    }

    public class SayTask : TaskNode
    {
        private readonly string _dialogue;
        private readonly float _duration;

        public SayTask(string dialogue, float duration = 3f)
        {
            _dialogue = dialogue;
            _duration = duration;
        }

        public override IEnumerator Execute(BOSSBlackboard blackboard, Action<NodeState> callback)
        {
            var npc = blackboard.Get<NPC>("Self");
            if (npc == null)
            {
                callback(NodeState.Failure);
                yield break;
            }

            MelonLogger.Msg($"[BOSSFramework] {npc.name} saying '{_dialogue}' for {_duration} seconds");
            var renderer = BOSSUtils.GetWorldspaceDialogueRenderer(npc);
            if (renderer != null)
            {
                renderer.ShowText(_dialogue, _duration);
            }
            try { npc.PlayVO(Il2CppScheduleOne.VoiceOver.EVOLineType.Acknowledge); } catch { }
            yield return null;
            callback(NodeState.Success);
        }
    }

    public class WaitTask : TaskNode
    {
        private readonly float _seconds;

        public WaitTask(float seconds) => _seconds = seconds;

        public override IEnumerator Execute(BOSSBlackboard blackboard, Action<NodeState> callback)
        {
            var npc = blackboard.Get<NPC>("Self");
            if (npc == null)
            {
                callback(NodeState.Failure);
                yield break;
            }

            MelonLogger.Msg($"[BOSSFramework] {npc.name} waiting (paused) for {_seconds} seconds");

            npc.Movement.PauseMovement();

            yield return new WaitForSeconds(_seconds);

            npc.Movement.ResumeMovement();
            callback(NodeState.Success);
        }
    }

    public class FollowNPCTask : TaskNode
    {
        private readonly float _distance;

        public FollowNPCTask(float distance = 3f) => _distance = distance;

        public override IEnumerator Execute(BOSSBlackboard blackboard, Action<NodeState> callback)
        {
            var npc = blackboard.Get<NPC>("Self");
            var target = blackboard.Get<NPC>("TargetNPC");
            if (npc == null || target == null)
            {
                callback(NodeState.Failure);
                yield break;
            }

            float dist = Vector3.Distance(npc.transform.position, target.transform.position);

            if (dist > _distance)
            {
                npc.Movement.SetDestination(target.transform.position);
                npc.Movement.ResumeMovement();
                callback(NodeState.Running); // Keep ticking this task
            }
            else
            {
                npc.Movement.PauseMovement(); // Optional: stop if close enough
                callback(NodeState.Success);
            }
        }
    }
    public class FollowPlayerTask : TaskNode
    {
        private readonly float _distance;

        public FollowPlayerTask(float distance = 3f)
        {
            _distance = distance;
        }

        public override IEnumerator Execute(BOSSBlackboard blackboard, Action<NodeState> callback)
        {
            var npc = blackboard.Get<NPC>("Self");
            var player = blackboard.Get<Player>("Player");

            if (npc == null || player == null)
            {
                callback(NodeState.Failure);
                yield break;
            }

            float dist = Vector3.Distance(npc.transform.position, player.transform.position);

            if (dist > _distance)
            {
                npc.Movement.SetDestination(player.transform.position);
                npc.Movement.ResumeMovement();
                callback(NodeState.Running); // Keep ticking this task
            }
            else
            {
                npc.Movement.PauseMovement(); // Optional: stop if close enough
                callback(NodeState.Success);
            }

            yield break;
        }
    }

}
