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

### 3. Create & Register a Custom Behavior
```csharp
public static IEnumerator SpinInPlace(NPC npc)
{
    while (true)
    {
        npc.transform.Rotate(0f, 90f * Time.deltaTime, 0f);
        yield return null;
    }
}

// Register when you want to, preferably on game load.
// Register(string id, Func<NPC, System.Collections.IEnumerator> start, [Action<NPC> stop = null])
BehaviorRegistry.Register("Spin", SpinInPlace, CustomStopFunction);
```

### 4. Start Your New Behavior on an NPC
```csharp
BehaviorRegistry.Start(npc, "Spin");
```


## 📁Project Structure

| File                | Purpose                                                         |
|---------------------|-----------------------------------------------------------------|
| `Core.cs`           | Initializes the framework and hooks into MelonLoader            |
| `BehaviorRegistry.cs` | Manages behavior registration, tracking, and stopping         |
| `CommonBehaviors.cs` | Example reusable behaviors like `FollowNearestPlayer`          |
| `CommonActions.cs`   | Coroutine helpers like `MoveTo`, `Wait`, `Say`                 |
| `BOSSUtils.cs`       | Recursive transform search, logging, and renderer helpers      |


## 🤝Contributing

Pull requests welcome! Share behaviors or extend functionality. CommonBehaviors is a great place to start.

## 📄License

MIT License — free to use, extend, and distribute.

Made with ❤️ by RidingKeys
