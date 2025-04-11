// BOSSFramework - BOSSUtils.cs
// Helper utilities for difficult problems.
using UnityEngine;
using Il2CppScheduleOne.NPCs;
using MelonLoader;
using System.Collections;

namespace BOSSFramework
{
    public static class BOSSUtils
    {
        public static Il2CppScheduleOne.NPCs.Behaviour.Behaviour IdleTemplate;
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

        public static Il2CppScheduleOne.UI.WorldspaceDialogueRenderer GetWorldspaceDialogueRenderer(NPC npc)
        {
            var spine2 = FindChildRecursive(npc.Avatar.BodyContainer.transform, "mixamorig:Spine2");
            if (spine2 != null)
            {
                var renderer = spine2.GetComponentInChildren<Il2CppScheduleOne.UI.WorldspaceDialogueRenderer>();
                if (renderer != null)
                {
                    MelonLoader.MelonLogger.Msg($"[BOSSFramework] Found WorldspaceDialogueRenderer on {npc.name}");
                    return renderer;
                }
                else
                {
                    MelonLoader.MelonLogger.Warning($"[BOSSFramework] No WorldspaceDialogueRenderer found under Spine2 on {npc.name}");
                }
            }
            return null;
        }
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
                            IdleTemplate = UnityEngine.Object.Instantiate(b);
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
