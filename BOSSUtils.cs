// BOSSUtils.cs
using UnityEngine;
using Il2CppScheduleOne.NPCs;

namespace BOSSFramework
{
    public static class BOSSUtils
    {
        public static Transform FindChildRecursive(Transform parent, string name)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                //MelonLoader.MelonLogger.Msg($"[BOSSFramework] Searching child '{child.name}' of parent '{parent.name}'");

                if (child.name == name)
                {
                    //MelonLoader.MelonLogger.Msg($"[BOSSFramework] Found child '{name}' under '{parent.name}'");
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
            //MelonLoader.MelonLogger.Msg($"[BOSSFramework] Looking for '{childName}' under '{parent.name}'");
            var child = FindChildRecursive(parent, childName);
            if (child != null)
            {
                //MelonLoader.MelonLogger.Msg($"[BOSSFramework] Found child '{childName}', checking for component '{typeof(T).Name}'");
                var component = child.GetComponent<T>();
                if (component != null)
                {
                    //MelonLoader.MelonLogger.Msg($"[BOSSFramework] Found component '{typeof(T).Name}' on '{child.name}'");
                    return component;
                }
                else
                {
                    //MelonLoader.MelonLogger.Warning($"[BOSSFramework] Component '{typeof(T).Name}' not found on '{child.name}'");
                }
            }
            else
            {
                //MelonLoader.MelonLogger.Warning($"[BOSSFramework] Child '{childName}' not found under '{parent.name}'");
            }
            return null;
        }

        public static Il2CppScheduleOne.UI.WorldspaceDialogueRenderer GetWorldspaceDialogueRenderer(NPC npc)
        {
            //MelonLoader.MelonLogger.Msg($"[BOSSFramework] Searching for Spine2 on {npc.name}...");
            var spine2 = FindChildRecursive(npc.Avatar.BodyContainer.transform, "mixamorig:Spine2");
            if (spine2 != null)
            {
                //MelonLoader.MelonLogger.Msg($"[BOSSFramework] Found Spine2 on {npc.name}: {spine2.name}");
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
     }
}
