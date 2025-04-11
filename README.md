# BOSSFramework

**BOSSFramework** (Behavioral Overlay System for Schedule I) is a custom modding framework for [Schedule I](https://store.steampowered.com/app/3164500/Schedule_I/) built on **MelonLoader** and **Harmony**. It provides a reusable, extensible system for controlling NPC behavior via coroutine-based behavior trees that can be assigned, paused, or removed dynamically at runtime.

---

## ✨ Features

- 🔁 Reusable behavior system with coroutine-based execution
- 🧠 Behavior Trees with Tasks, Conditions, Selectors, and Sequences
- 🔧 Dual Runtime Support — compatible with both Mono and IL2CPP
- 🧱 Interface-driven backend abstraction for clean mod interoperability

---

## 🛠 Requirements

- [MelonLoader](https://melonwiki.xyz/#/) (compatible with both Mono & IL2CPP versions of Schedule I)
- Your mod must reference `BOSSCoreShared.dll` and **either** `BOSSMono.dll` **or** `BOSSIl2Cpp.dll` depending on the runtime version you're targeting.

> 💡 IL2CPP and Mono versions **must not** be used interchangeably. Only drop the correct backend DLLs into your `Mods/` folder based on which version of the game you're running.

---

## 🚀 Getting Started

### 1. Drop DLLs into the `Mods/` folder for your Schedule I install:

**Mono Version:**
```
Mods/
├── BOSSCoreShared.dll
├── BOSSMono.dll
```

**IL2CPP Version:**
```
Mods/
├── BOSSCoreShared.dll
├── BOSSIl2Cpp.dll
```
---

## 👩‍💻 For Modders: Using BOSSFramework in Your Mod

### ✅ 1. Add the Correct References
Target **one** backend based on the Schedule I version you are modding for:

- **Mono:** `BOSSCoreShared.dll` and `BOSSMono.dll`
- **IL2CPP:** `BOSSCoreShared.dll` and `BOSSIl2Cpp.dll`

> ⚠️ Do NOT reference both backend DLLs in the same project.

### ✅ 2. Only Code Against Shared Interfaces
Use the shared types provided by `BOSSCoreShared`:
```csharp
using BOSSCoreShared;
using BOSSFramework.BehaviorTree;
```
Use `INPC`, `IPlayer`, `IBehavior`, and `BehaviorRegistry`. Let the framework handle the backend wiring.

### ✅ 3. Avoid Backend Implementations
Do **not** use or reference:
- `MonoNPC`, `Il2CppNPC`
- `MonoPlayer`, `Il2CppPlayer`
- `MonoBehavior`, `Il2CppBehavior`

Those are managed internally and are runtime-specific.

### ✅ 4. Example Behavior Tree Usage
```csharp
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

// Applying it:
BehaviorRegistry.Apply(npc, "MyTree", MyCustomTree.Create(npc, player));
```

---

---

## 📁 Project Structure

| Path | Description |
|------|-------------|
| 📁 /Mono | Mono-specific interfaces for Schedule I objects (NPC, Player, Dialogue). |
| 📁 /Il2Cpp | IL2CPP-specific implementations of the same interfaces. |
| 📁 /CoreShared | Interface contracts and shared hooks between Mono and Il2Cpp. |
| 📁 Examples | Example behavior tree (`BTExampleTree.cs`) and usage patterns. |

---

## 🤝 Contributing

Pull requests welcome! The best way to contribute is by creating:
- New `BTTasks` or `BTConditions`
- Shareable tree templates via `BTExampleTree`-style setups
- Utility components for common AI behaviors

---

## 📄 License

MIT License — free to use, extend, and distribute.

Made with ❤️ by RidingKeys
