# BOSSFramework

**BOSSFramework** (Behavioral Overlay System for Schedule I) is a custom modding framework for [Schedule I](https://store.steampowered.com/app/2585950/Schedule_1/) built on **MelonLoader** and **Harmony**. It provides a reusable, extensible system for controlling NPC behavior via coroutine-based "behaviors" that can be assigned, paused, or stopped dynamically.

---

## ✨ Features

- 🔁 Reusable behavior system with coroutine-based execution
- 🧠 Register custom and complex behavior trees per NPC
- 🔧 Easily expandable by other mods

---

## 🛠 Requirements

- [MelonLoader](https://melonwiki.xyz/#/) (for Schedule I)
- IL2CPP game version of **Schedule I**
- Your mod project must reference `BOSSFramework.dll` to use the API

---

## 🚀 Getting Started

### 1. Drop `BOSSFramework.dll` into your `Mods/` folder.

### 2. In your own mod, reference it:

```csharp
using BOSSFramework;
```

### 3. Creating a Custom Behavior Tree
Behavior Trees in BOSSFramework use a builder-style DSL to create modular logic based on Tasks, Conditions, and composite Nodes (Sequences and Selectors). Each behavior tree runs on a per-NPC basis and operates using a Blackboard system for shared context.

✨ Example: BTExampleTree.cs
```csharp
// BOSSFramework - BTExampleTree.cs
// Example behavior tree using core Tasks and Nodes for testing

using Il2CppScheduleOne.NPCs;
using Il2CppScheduleOne.PlayerScripts;
using BOSSFramework.BehaviorTree;
using BOSSFramework.BehaviorTree.Tasks;

namespace BOSSFramework
{
    public static class BTExampleTree
    {
        public static BehaviorTree.BehaviorTree Create(NPC npc, Player player)
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
```

## 4. Applying and Removing a Behavior Tree
BOSSFramework does not use string ID registrations anymore. Instead, trees are applied directly to an NPC at runtime with a simple method call.

✅ Applying a Behavior Tree
```csharp
var npc = FindClosestNPCToPlayer();
var player = GameObject.Find("Player_Local").GetComponent<Player>();

var tree = BTExampleTree.Create(npc, player);
BehaviorRegistry.Apply(npc, "FollowExampleTree", tree);
```
>The second parameter ("FollowExampleTree") is used for logging/debugging and distinguishing which behavior is currently running.

🛑 Stopping a Tree
```csharp
BehaviorRegistry.Remove(npc);
```
🔁 Stopping All Behavior Trees
```csharp
BehaviorRegistry.RemoveAll();
```
>This is automatically called when returning to the main menu.

## 📁 Project Structure

| Path | Description |
|------|-------------|
|├─ 📄 Core.cs | Entry point of the BOSSFramework. Handles setup, Harmony patches, and behavior lifecycle management. |
|├─ 📁 BehaviorTree | Core behavior tree logic — nodes, blackboard, conditions, and structure. |
|│  ├─ 📄 BehaviorTree.cs | Base class and logic for BehaviorTrees. |
|│  ├─ 📄 BTNodes.cs | Base class and logic for all nodes (`TaskNode`, `SelectorNode`, `SequenceNode`). |
|│  ├─ 📄 BTBuilders.cs | Builder-style helper classes for composing behavior trees fluently. |
|│  └─ 📁 Tasks | Contains various tasks & conditions for use in BehaviorTrees. |
|│     ├─ 📄 BTConditions.cs | Contains modular conditional nodes like distance checks and cooldown timers. |
|│     └─ 📄 BTTasks.cs | Core task actions for NPCs (e.g. `SayTask`, `WaitTask`, `FollowPlayerTask`). |
|├─ 📁 Examples| Example implementations |
|│  └─ 📄 BTExampleTree.cs | Example implementation of a working `BehaviorTree` with player interaction logic. |
|├─ 📁 Utils| Helper classes |
|│  ├─ 📄 BOSSUtils.cs | Utility functions for logging, finding components, and dialogue helpers. |
|│  └─ 📄 BehaviorRegistry.cs | Applies, tracks, and removes `BehaviorTree` instances per NPC. Handles pausing/resuming original game behaviors. |



## 🤝Contributing

Pull requests welcome! Share behaviors or extend functionality. BTTasks & BTExampleTree are great examples to start working with!

## 📄License

MIT License — free to use, extend, and distribute.

Made with ❤️ by RidingKeys
