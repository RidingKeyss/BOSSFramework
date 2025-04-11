// MONO-specific startup logic and hooks
using UnityEngine.Events;
using MelonLoader;
using HarmonyLib;
using System.Collections;
using static BOSSCoreShared.BOSSUtils;
using BOSSCoreShared;
using ScheduleOne.Persistence;
using BOSSFramework.BehaviorTree;
using BOSSFramework.Mono;

namespace BOSSMono
{
    public static class MonoInitHooks
    {
        public static void Initialize()
        {
            if (MelonUtils.IsGameIl2Cpp())
            {
                MelonLogger.Warning("[BOSSFramework] Skipping MonoInitHooks: not running in Mono.");
                return;
            }

            var harmony = new HarmonyLib.Harmony("BOSSMono");
            var type = typeof(LoadManager);
            var method = AccessTools.Method(type, "ExitToMenu");
            var prefix = AccessTools.Method(typeof(ExitToMenu_Patch), nameof(ExitToMenu_Patch.Prefix));

            if (method != null && prefix != null)
            {
                harmony.Patch(method, prefix: new HarmonyMethod(prefix));
                MelonLogger.Msg("[BOSSFramework] Mono Harmony patch applied.");
            }
            else
            {
                MelonLogger.Warning("[BOSSFramework] Failed to apply Mono Harmony patch.");
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
            LoadManager.Instance.onLoadComplete.AddListener(OnLoadComplete);
        }

        private static void OnLoadComplete()
        {
            MelonLogger.Msg("[BOSSFramework] Loading completed.");
            MonoDebugTools.RegisterDebugKeys();
            var provider = new MonoIdleTemplateProvider();
            BackendHooks.IdleTemplateProvider = provider;
            IdleTemplate = provider.GetIdleTemplate();
            BackendHooks.BehaviorCloner = new MonoBehaviorCloner();
        }

        public static class ExitToMenu_Patch
        {
            public static void Prefix()
            {
                MelonLogger.Msg("[BOSSFramework] [Mono] Exiting to menu — clearing custom behaviors.");
                BehaviorRegistry.RemoveAll();
            }
        }
    }
}
