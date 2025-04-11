﻿// BOSSFramework - BehaviorTree.cs
// BehaviorTree container for modders to build and run custom logic per-NPC

using System.Collections;
using Il2CppScheduleOne.NPCs;

namespace BOSSFramework.BehaviorTree
{
    public class BehaviorTree
    {
        private readonly BTNode _root;
        private readonly BOSSBlackboard _blackboard = new();
        private bool _running = true;

        public BehaviorTree(BTNode root)
        {
            _root = root;
        }

        public IEnumerator Run()
        {
            while (_running)
            {
                var npc = _blackboard.Get<NPC>("Self");

                if (npc == null || npc.gameObject == null)
                    yield break;

                bool finished = false;
                NodeState result = NodeState.Failure;

                yield return _root.Execute(_blackboard, state =>
                {
                    result = state;
                    finished = true;
                });

                while (!finished)
                    yield return null;

                yield return null; // Optional tick delay
            }
        }

        public void Stop() => _running = false;

        public BOSSBlackboard GetBlackboard() => _blackboard;
    }
}
