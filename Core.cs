// BOSSFramework - Behavior Overlay System for Schedule One
// Compatible with MelonLoader & Harmony - IL2CPP only

using MelonLoader;
using BOSSFramework.Backends.Il2Cpp;
using static BOSSFramework.Shared.BOSSUtils;

[assembly: MelonInfo(typeof(BOSSFramework.Core), "BOSSFramework", "1.0.6", "RidingKeys")]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace BOSSFramework
{
    public class Core : MelonMod
    {
        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("BOSSFramework initialized. Ready for custom NPC behaviors.");
            BackendHooks.IsIl2Cpp = MelonUtils.IsGameIl2Cpp();
            if (BackendHooks.IsIl2Cpp)
            {
                Il2CppInitHooks.Initialize();
            }
        }
    }
}
