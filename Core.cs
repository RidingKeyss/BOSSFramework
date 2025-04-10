// BOSSFramework - Schedule I Custom NPC Behavior Modding Framework
// Compatible with MelonLoader & Harmony - IL2CPP only

using UnityEngine;
using MelonLoader;
using HarmonyLib;
using Il2CppScheduleOne.NPCs;
using Il2CppScheduleOne.Persistence;
using Il2CppScheduleOne.DevUtilities;

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

        [HarmonyPatch(typeof(GameManager), "Load")]
        public static class GameManager_Load_Patch
        {
            public static void Postfix()
            {
                MelonLogger.Msg("[BOSSFramework] Registering behaviors.");

                BehaviorRegistry.Register("MoveToRandom", CommonBehaviors.MoveToRandom, CommonBehaviors.StopMovement);
                BehaviorRegistry.Register("FollowNearestNPC", CommonBehaviors.FollowNearestNPC, CommonBehaviors.StopMovement);
                BehaviorRegistry.Register("FollowNearestPlayer", CommonBehaviors.FollowNearestPlayer, CommonBehaviors.StopMovement);
            }
        }

        [HarmonyPatch(typeof(LoadManager), "ExitToMenu")]
        public static class LoadManager_ExitToMenu_Patch
        {
            public static void Prefix()
            {
                MelonLogger.Msg("[BOSSFramework] Exiting to menu — clearing custom behaviors.");
                BehaviorRegistry.StopAll();
            }
        }
    }
}
