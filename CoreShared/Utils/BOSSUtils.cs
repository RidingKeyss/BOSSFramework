// BOSSFramework - Utils/BOSSUtils.cs
// Helper utilities for difficult problems.
using UnityEngine;

namespace BOSSCoreShared
{
    public static class BOSSUtils
    {
        public static class BackendHooks
        {
            public static IIdleTemplateProvider IdleTemplateProvider;
            public static bool IsIl2Cpp { get; set; }
            public static IBehaviorCloner? BehaviorCloner;
            public static bool DebugModeEnabled()
            {
                return Environment.GetCommandLineArgs().Any(arg => arg.Equals("--bossdebug", StringComparison.OrdinalIgnoreCase));
            }

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
    }
}
