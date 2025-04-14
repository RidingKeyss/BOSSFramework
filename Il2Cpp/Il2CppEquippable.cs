// BOSSFramework - Il2Cpp/Il2CppEquippable.cs
// Wrapper for IL2CPP AvatarEquippable objects

using Il2CppScheduleOne.AvatarFramework.Equipping;
using BOSSCoreShared;
using UnityEngine;

namespace BOSSIl2Cpp
{
    public class Il2CppEquippable : IEquippable
    {
        protected readonly AvatarEquippable Wrapped;

        public Il2CppEquippable(AvatarEquippable equippable)
        {
            Wrapped = equippable;
        }

        public GameObject GameObject => Wrapped?.gameObject;
        public string Path => Wrapped?.AssetPath;
    }
}
