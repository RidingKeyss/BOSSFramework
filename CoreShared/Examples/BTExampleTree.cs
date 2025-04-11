// BOSSFramework - Examples/BTExampleTree.cs
// Example behavior tree using core Tasks and Nodes for testing

using BOSSFramework.BehaviorTree;
using BOSSFramework.BehaviorTree.Tasks;
using BOSSCoreShared;

namespace BOSSFramework.Examples
{
    public static class BTExampleTree
    {
        public static BehaviorTree.BehaviorTree Create(INPC npc, IPlayer player)
        {
            var tree = new BehaviorTree.BehaviorTree(
                SelectorNodeBuilder.Start()
                // Sequence 1: Player is very close, engage
                .WithSequence(
                    new IsPlayerNearCondition(6f),
                    new ConditionalTaskNode(
                        new CooldownCondition("spotted", 6f),
                        new SayTask("I see you...", 2f)
                    ),
                    SelectorNodeBuilder.Start()
                        .WithSequence(
                            new IsPlayerNearCondition(2f),
                            new ConditionalTaskNode(
                                new CooldownCondition("gotcha", 10f),
                                new SayTask("Gotcha!", 2f)
                            )
                        )
                        .WithSequence(
                            new FollowPlayerTask(1f)
                        )
                        .Build()
                )

                // Sequence 2: Passive idle when player is somewhat close
                .WithSequence(
                    new IsPlayerNearCondition(15f),
                    new ConditionalTaskNode(
                        new CooldownCondition("chillin", 8f),
                        new SayTask("Just hanging out", 2f)
                    ),
                    new WaitTask(0.5f)
                )

                // Sequence 3: Do nothing (pure idle)
                .WithSequence(
                    new WaitTask(0.5f)
                )

                .Build()
            );

            tree.GetBlackboard().Set("Self", npc);
            tree.GetBlackboard().Set("Player", player);

            return tree;
        }
    }

}
