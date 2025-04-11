// BOSSFramework - Behavior Overlay System for Schedule One
// Compatible with MelonLoader & Harmony - IL2CPP only

using MelonLoader;
using HarmonyLib;
using Il2CppScheduleOne.Persistence;
using Il2CppScheduleOne.PlayerScripts;
using UnityEngine.Events;
using Il2CppScheduleOne.NPCs;
using UnityEngine;

[assembly: MelonInfo(typeof(BOSSFramework.Core), "BOSSFramework", "1.0.0", "RidingKeys")]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace BOSSFramework
{
    public class Core : MelonMod
    {
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("BOSSFramework initialized. Ready for custom NPC behaviors.");
        }

        [HarmonyPatch(typeof(LoadManager), "ExitToMenu")]
        public static class LoadManager_ExitToMenu_Patch
        {
            public static void Prefix()
            {
                MelonLogger.Msg("[BOSSFramework] Exiting to menu — clearing custom behaviors.");
                BehaviorRegistry.RemoveAll();
            }
        }

        void Start()
        {
            if (LoadManager.Instance != null)
            {
                LoadManager.Instance.onLoadComplete.AddListener((UnityAction)OnLoadComplete);
            }
        }

        void OnLoadComplete()
        {
            BOSSUtils.CacheIdleTemplate();
        }
    }
}
