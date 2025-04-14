// BOSSFramework - BehaviorTree/Tasks/BTTasks.cs
// Implements reusable TaskNodes for core NPC actions

using System.Collections;
using UnityEngine;
using MelonLoader;
using BOSSCoreShared;

namespace BOSSFramework.BehaviorTree.Tasks
{
    public class MoveToTask : TaskNode
    {
        public override IEnumerator Execute(BOSSBlackboard blackboard, Action<NodeState> callback)
        {
            var npc = blackboard.Get<INPC>("Self");
            var target = blackboard.Get<Vector3>("Position");
            if (npc == null || !blackboard.Has("Position"))
            {
                callback(NodeState.Failure);
                yield break;
            }

            if (npc.CanMove())
            {
                if (npc.GetClosestReachablePoint(target, out Vector3 reachable))
                {
                    npc.SetDestination(reachable);
                    npc.ResumeMovement();
                    MelonLogger.Msg($"[BOSSFramework] {npc.Name} moving to {reachable}");

                    while (npc.IsMoving)
                    {
                        callback(NodeState.Running);
                    }

                    callback(NodeState.Success);
                    yield break;
                }
                else
                {
                    MelonLogger.Msg($"[BOSSFramework] {npc.Name} could not find path to destination.");
                }
            }
            else
            {
                if (npc.IsInBuilding) npc.ExitBuilding();
                if (npc.IsInVehicle) npc.ExitVehicle();
                npc.PauseMovement();
                MelonLogger.Msg($"[BOSSFramework] {npc.Name} cannot move.");
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
            var npc = blackboard.Get<INPC>("Self");
            if (npc == null)
            {
                callback(NodeState.Failure);
                yield break;
            }

            MelonLogger.Msg($"[BOSSFramework] {npc.Name} saying '{_dialogue}' for {_duration} seconds");
            var renderer = npc.GetDialogueRenderer();
            if (renderer != null)
            {
                renderer.ShowText(_dialogue, _duration);
            }
            try { npc.PlayVoiceLine(VoiceLineType.Acknowledge); } catch { }
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
            var npc = blackboard.Get<INPC>("Self");
            if (npc == null)
            {
                callback(NodeState.Failure);
                yield break;
            }

            MelonLogger.Msg($"[BOSSFramework] {npc.Name} waiting (paused) for {_seconds} seconds");

            npc.PauseMovement();

            yield return new WaitForSeconds(_seconds);

            npc.ResumeMovement();
            callback(NodeState.Success);
        }
    }

    public class FollowNPCTask : TaskNode
    {
        private readonly float _distance;

        public FollowNPCTask(float distance = 3f) => _distance = distance;

        public override IEnumerator Execute(BOSSBlackboard blackboard, Action<NodeState> callback)
        {
            var npc = blackboard.Get<INPC>("Self");
            var target = blackboard.Get<INPC>("TargetNPC");
            if (npc == null || target == null)
            {
                callback(NodeState.Failure);
                yield break;
            }

            float dist = Vector3.Distance(npc.Transform.position, target.Transform.position);

            if (dist > _distance)
            {
                npc.SetDestination(target.Transform.position);
                npc.ResumeMovement();
                callback(NodeState.Running); // Keep ticking this task
            }
            else
            {
                npc.PauseMovement(); // Optional: stop if close enough
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
            var npc = blackboard.Get<INPC>("Self");
            var player = blackboard.Get<IPlayer>("Player");

            if (npc == null || player == null)
            {
                callback(NodeState.Failure);
                yield break;
            }

            float dist = Vector3.Distance(npc.Transform.position, player.Transform.position);

            if (dist > _distance)
            {
                npc.SetDestination(player.Transform.position);
                npc.ResumeMovement();
                callback(NodeState.Running); // Keep ticking this task
            }
            else
            {
                npc.PauseMovement(); // Optional: stop if close enough
                callback(NodeState.Success);
            }

            yield break;
        }
    }

    public class AttackTask : TaskNode
    {
        public override IEnumerator Execute(BOSSBlackboard blackboard, Action<NodeState> callback)
        {
            var npc = blackboard.Get<INPC>("Self");
            var player = blackboard.Has("Player") ? blackboard.Get<IPlayer>("Player") : null;
            var targetNpc = blackboard.Has("TargetNPC") ? blackboard.Get<INPC>("TargetNPC") : null;

            if (npc == null)
            {
                callback(NodeState.Failure);
                yield break;
            }

            var weapon = blackboard.Has("Weapon") ? blackboard.Get<IAvatarWeapon>("Weapon") : null;
            if (weapon == null)
            {
                MelonLogger.Warning("[BOSSFramework] No weapon assigned on blackboard.");
                callback(NodeState.Failure);
                yield break;
            }

            if (weapon is IAvatarRangedWeapon ranged && player != null)
            {
                if (ranged.IsPlayerInLoS(player))
                {
                    ranged.SetIsRaised(true);

                    float accuracy = Mathf.Clamp01(npc.Accuracy);
                    float maxSpread = Mathf.Lerp(15f, 0.5f, accuracy);

                    Vector3 spread = new Vector3(
                        UnityEngine.Random.Range(-maxSpread, maxSpread),
                        UnityEngine.Random.Range(-maxSpread, maxSpread),
                        UnityEngine.Random.Range(-maxSpread, maxSpread)
                    );

                    Vector3 shotPosition = player.Transform.position + spread;

                    ranged.Shoot(shotPosition);
                    callback(NodeState.Success);
                    yield break;
                }
            }

            weapon.Attack();
            yield return new WaitForSeconds(1f);
            callback(NodeState.Success);
        }
    }
}
