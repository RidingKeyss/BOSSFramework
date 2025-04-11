// BOSSFramework - Utils/BOSSUtils.cs
// Helper utilities for difficult problems.
using UnityEngine;
using Il2CppScheduleOne.NPCs;
using MelonLoader;

namespace BOSSFramework.Shared
{
    public static class BOSSUtils
    {
        public static class BackendHooks
        {
            public static IIdleTemplateProvider IdleTemplateProvider;
            public static bool IsIl2Cpp { get; set; }
        }

        public static IBehavior IdleTemplate;
        public static Transform FindChildRecursive(Transform parent, string name)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if (child.name == name)
                {
                    return child;
                }
                var result = FindChildRecursive(child, name);
                if (result != null)
                    return result;
            }

            return null;
        }

        public static T FindComponentInChildren<T>(Transform parent, string childName) where T : Component
        {
            var child = FindChildRecursive(parent, childName);
            if (child != null)
            {
                var component = child.GetComponent<T>();
                if (component != null)
                {
                    return component;
                }
            }
            return null;
        }

        [System.Obsolete("Use INPC.GetDialogueRenderer() instead.")]
        public static Il2CppScheduleOne.UI.WorldspaceDialogueRenderer GetWorldspaceDialogueRenderer(INPC npc)
        {
            var spine2 = FindChildRecursive(npc.AvatarRoot, "mixamorig:Spine2");
            if (spine2 != null)
            {
                var renderer = spine2.GetComponentInChildren<Il2CppScheduleOne.UI.WorldspaceDialogueRenderer>();
                if (renderer != null)
                {
                    MelonLoader.MelonLogger.Msg($"[BOSSFramework] Found WorldspaceDialogueRenderer on {npc.Name}");
                    return renderer;
                }
                else
                {
                    MelonLoader.MelonLogger.Warning($"[BOSSFramework] No WorldspaceDialogueRenderer found under Spine2 on {npc.Name}");
                }
            }
            return null;
        }
        [System.Obsolete("This method is IL2CPP-specific and should be moved to an IL2CPP backend module.")]
        public static void CacheIdleTemplate()
        {
            var npcs = GameObject.FindObjectsOfType<NPC>();
            foreach (var npc in npcs)
            {
                foreach (var b in npc.behaviour.behaviourStack)
                {
                    if (b != null)
                    {
                        //MelonLogger.Msg($"[BOSSFramework] Found behaviour type: {b.GetIl2CppType().ToString()}");
                        if (b.GetIl2CppType().Name == "StationaryBehaviour")
                        {
                            //IdleTemplate = UnityEngine.Object.Instantiate(b);
                            MelonLogger.Msg("[BOSSFramework] StationaryBehaviour template cloned and cached.");
                            return;
                        }
                    }
                }
            }

            MelonLogger.Warning("[BOSSFramework] Could not find any StationaryBehaviour to use as a fallback template.");
        }
    }
}
