// IL2CPP-specific startup logic and hooks
using UnityEngine.Events;
using Il2CppScheduleOne.Persistence;
using static BOSSFramework.Shared.BOSSUtils;
using MelonLoader;
using HarmonyLib;
using BOSSFramework.Shared;
using System.Collections;

namespace BOSSFramework.Backends.Il2Cpp
{
    public static class Il2CppInitHooks
    {
        [HarmonyPatch(typeof(LoadManager), "ExitToMenu")]
        public static class LoadManager_ExitToMenu_Patch
        {
            public static void Prefix()
            {
                MelonLogger.Msg("[BOSSFramework] Exiting to menu — clearing custom behaviors.");
                Shared.BehaviorRegistry.RemoveAll();
            }
        }

        public static void Initialize()
        {
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
            BOSSUtils.IdleTemplate = provider.GetIdleTemplate();
        }
    }
}
