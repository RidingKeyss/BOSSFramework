// IL2CPP-specific startup logic and hooks
using UnityEngine.Events;
using MelonLoader;
using HarmonyLib;
using System.Collections;
using static BOSSCoreShared.BOSSUtils;
using BOSSCoreShared;
using Il2CppScheduleOne.Persistence;
using BOSSFramework.BehaviorTree;

namespace BOSSIl2Cpp
{
    public static class Il2CppInitHooks
    {
        public static void Initialize()
        {
            if (!MelonUtils.IsGameIl2Cpp())
            {
                MelonLogger.Warning("[BOSSFramework] Skipping Il2CppInitHooks: not running in IL2CPP.");
                return;
            }

            // Manually apply Harmony patch
            var harmony = new HarmonyLib.Harmony("BOSSIl2Cpp");
            var type = typeof(LoadManager);
            var method = AccessTools.Method(type, "ExitToMenu");
            var prefix = AccessTools.Method(typeof(ExitToMenu_Patch), nameof(ExitToMenu_Patch.Prefix));

            if (method != null && prefix != null)
            {
                harmony.Patch(method, prefix: new HarmonyMethod(prefix));
                MelonLogger.Msg("[BOSSFramework] Il2Cpp Harmony patch applied.");
            }
            else
            {
                MelonLogger.Warning("[BOSSFramework] Failed to apply Il2Cpp Harmony patch.");
            }

            MelonCoroutines.Start(WaitForLoadManager());
        }

        private static IEnumerator WaitForLoadManager()
        {
            while (LoadManager.Instance == null)
            {
                yield return null;
            }

            MelonLogger.Msg("[BOSSFramework] LoadManager detected. Registering onLoadComplete...");
            LoadManager.Instance.onLoadComplete.AddListener((UnityAction)OnLoadComplete);
        }

        private static void OnLoadComplete()
        {
            MelonLogger.Msg("[BOSSFramework] Loading completed.");
            Il2CppDebugTools.RegisterDebugKeys();
            var provider = new Il2CppIdleTemplateProvider();
            BackendHooks.IdleTemplateProvider = provider;
            IdleTemplate = provider.GetIdleTemplate();
            BackendHooks.BehaviorCloner = new Il2CppBehaviorCloner();
        }

        public static class ExitToMenu_Patch
        {
            public static void Prefix()
            {
                MelonLogger.Msg("[BOSSFramework] [Il2Cpp] Exiting to menu — clearing custom behaviors.");
                BehaviorRegistry.RemoveAll();
            }
        }
    }
}
