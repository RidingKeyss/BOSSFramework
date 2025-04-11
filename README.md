# BOSSFramework

**BOSSFramework** (Behavioral Overlay System for Schedule I) is a custom modding framework for [Schedule I](https://store.steampowered.com/app/2585950/Schedule_1/) built on **MelonLoader** and **Harmony**. It provides a reusable, extensible system for controlling NPC behavior via coroutine-based behavior trees that can be assigned, paused, or removed dynamically at runtime.

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

### 2. Reference the framework in your own mod:
```csharp
using BOSSFramework;
```

### 3. Creating a Custom Behavior Tree
Behavior Trees use a fluent builder API to assemble logic via tasks, conditions, and composite structures.

➡️ See `BTExampleTree.cs` for a working implementation.

### 4. Applying and Removing Behavior Trees
```csharp
var npc = FindClosestNPCToPlayer();
var player = GameObject.Find("Player_Local").GetComponent<Player>();

var tree = BTExampleTree.Create(npc, player);
BehaviorRegistry.Apply(npc, "FollowExampleTree", tree);
```

Remove a behavior:
```csharp
BehaviorRegistry.Remove(npc);
```

Remove all active behaviors:
```csharp
BehaviorRegistry.RemoveAll();
```

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
