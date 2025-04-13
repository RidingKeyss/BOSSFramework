// BOSSFramework - BehaviorTree/BTNodes.cs
// Defines node structures for creating modular, ticking behavior trees

using System.Collections;

namespace BOSSFramework.BehaviorTree
{
    public enum NodeState
    {
        Success,
        Failure,
        Running
    }

    public abstract class BTNode
    {
        public abstract IEnumerator Execute(BOSSBlackboard blackboard, Action<NodeState> callback);
    }

    public class BOSSBlackboard
    {
        private Dictionary<string, object> _data = new();

        public void Set<T>(string key, T value) => _data[key] = value;
        public T Get<T>(string key) => _data.TryGetValue(key, out var val) ? (T)val : default;
        public bool Has(string key) => _data.ContainsKey(key);
        public void Clear() => _data.Clear();

        public void InheritFrom(BOSSBlackboard other)
        {
            foreach (var kv in other._data)
            {
                if (!_data.ContainsKey(kv.Key))
                    _data[kv.Key] = kv.Value;
            }
        }
    }

    public class SequenceNode : BTNode
    {
        private readonly List<BTNode> _children;
        private int _currentIndex = 0;

        public SequenceNode(params BTNode[] nodes) => _children = new List<BTNode>(nodes);

        public override IEnumerator Execute(BOSSBlackboard blackboard, Action<NodeState> callback)
        {
            while (_currentIndex < _children.Count)
            {
                bool finished = false;
                NodeState result = NodeState.Failure;

                yield return _children[_currentIndex].Execute(blackboard, state =>
                {
                    result = state;
                    finished = true;
                });

                while (!finished) yield return null;

                if (result == NodeState.Failure)
                {
                    _currentIndex = 0;
                    callback(NodeState.Failure);
                    yield break;
                }

                if (result == NodeState.Running)
                {
                    callback(NodeState.Running);
                    yield break;
                }

                _currentIndex++;
            }

            _currentIndex = 0;
            callback(NodeState.Success);
        }
    }
    public class SelectorNode : BTNode
    {
        private readonly List<BTNode> _children;

        public SelectorNode(params BTNode[] nodes)
        {
            _children = new List<BTNode>(nodes);
        }

        public override IEnumerator Execute(BOSSBlackboard blackboard, Action<NodeState> callback)
        {
            foreach (var child in _children)
            {
                bool finished = false;
                NodeState result = NodeState.Failure;

                yield return child.Execute(blackboard, state =>
                {
                    result = state;
                    finished = true;
                });

                while (!finished)
                    yield return null;

                if (result == NodeState.Success)
                {
                    callback(NodeState.Success);
                    yield break;
                }

                if (result == NodeState.Running)
                {
                    callback(NodeState.Running);
                    yield break;
                }
            }

            callback(NodeState.Failure);
        }
    }

    public class ConditionalTaskNode : BTNode
    {
        private readonly BTNode _condition;
        private readonly TaskNode _task;

        public ConditionalTaskNode(BTNode condition, TaskNode task)
        {
            _condition = condition;
            _task = task;
        }

        public override IEnumerator Execute(BOSSBlackboard blackboard, Action<NodeState> callback)
        {
            NodeState condState = NodeState.Failure;
            bool finished = false;

            yield return _condition.Execute(blackboard, state =>
            {
                condState = state;
                finished = true;
            });

            while (!finished)
                yield return null;

            if (condState == NodeState.Success)
            {
                yield return _task.Execute(blackboard, callback);
            }
            else
            {
                callback(NodeState.Success); // Skipped task, but we didn't fail the whole sequence
            }
        }
    }

    public class NestedTreeNode : TaskNode
    {
        private readonly BehaviorTree _nestedTree;

        public NestedTreeNode(BehaviorTree nestedTree)
        {
            _nestedTree = nestedTree;
        }

        public override IEnumerator Execute(BOSSBlackboard blackboard, Action<NodeState> callback)
        {
            // Inject parent's blackboard into the nested tree
            _nestedTree.GetBlackboard().InheritFrom(blackboard);

            yield return _nestedTree.RunAsNode(result =>
            {
                callback(result);
            });
        }
    }


    public abstract class TaskNode : BTNode { }
}
