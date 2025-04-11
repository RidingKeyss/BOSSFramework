// BOSSFramework - BehaviorTree/Tasks/BTConditions.cs
// Base and reusable condition nodes for blackboard-driven behavior trees

using System.Collections;
using UnityEngine;
using BOSSCoreShared;

namespace BOSSFramework.BehaviorTree.Tasks
{
    public abstract class ConditionNode : TaskNode
    {
        public abstract bool CheckCondition(BOSSBlackboard blackboard);

        public override IEnumerator Execute(BOSSBlackboard blackboard, Action<NodeState> callback)
        {
            bool result = CheckCondition(blackboard);
            callback(result ? NodeState.Success : NodeState.Failure);
            yield break;
        }
    }

    public class IsPlayerNearCondition : ConditionNode
    {
        private readonly float _maxDistance;

        public IsPlayerNearCondition(float maxDistance)
        {
            _maxDistance = maxDistance;
        }

        public override bool CheckCondition(BOSSBlackboard blackboard)
        {
            var npc = blackboard.Get<INPC>("Self");
            var player = blackboard.Get<IPlayer>("Player");

            if (npc == null || player == null)
            {
                return false;
            }

            float dist = Vector3.Distance(npc.Transform.position, player.Transform.position);
            return dist <= _maxDistance;
        }
    }

    public class CooldownCondition : ConditionNode
    {
        private readonly string _key;
        private readonly float _cooldownSeconds;

        public CooldownCondition(string key, float cooldownSeconds)
        {
            _key = key;
            _cooldownSeconds = cooldownSeconds;
        }

        public override bool CheckCondition(BOSSBlackboard blackboard)
        {
            float currentTime = Time.time;
            float lastTime = blackboard.Get<float>(_key);

            if (currentTime - lastTime >= _cooldownSeconds)
            {
                blackboard.Set(_key, currentTime);
                return true;
            }

            return false;
        }
    }
}
