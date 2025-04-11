// BOSSFramework - BTBuilders.cs
// Builder pattern for cleanly constructing behavior tree node hierarchies

namespace BOSSFramework.BehaviorTree
{
    public class SelectorNodeBuilder
    {
        private readonly List<BTNode> _children = new();

        public static SelectorNodeBuilder Start() => new SelectorNodeBuilder();

        public SelectorNodeBuilder WithTask(BTNode task)
        {
            _children.Add(task);
            return this;
        }

        public SelectorNodeBuilder WithSequence(params BTNode[] tasks)
        {
            _children.Add(new SequenceNode(tasks));
            return this;
        }

        public SelectorNodeBuilder WithSelector(params BTNode[] branches)
        {
            _children.Add(new SelectorNode(branches));
            return this;
        }

        public SelectorNode Build() => new SelectorNode(_children.ToArray());
    }

    public class SequenceNodeBuilder
    {
        private readonly List<BTNode> _children = new();

        public static SequenceNodeBuilder Start() => new SequenceNodeBuilder();

        public SequenceNodeBuilder WithTask(BTNode task)
        {
            _children.Add(task);
            return this;
        }

        public SequenceNodeBuilder WithSequence(params BTNode[] tasks)
        {
            _children.Add(new SequenceNode(tasks));
            return this;
        }

        public SequenceNodeBuilder WithSelector(params BTNode[] branches)
        {
            _children.Add(new SelectorNode(branches));
            return this;
        }

        public SequenceNode Build() => new SequenceNode(_children.ToArray());
    }
}
