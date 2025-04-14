// BOSSFramework - Mono/MonoEquippable.cs
// Wrapper for Mono AvatarEquippable objects

using UnityEngine;
using BOSSCoreShared;

namespace BOSSMono
{
    public class MonoEquippable : IEquippable
    {
        protected readonly object Wrapped;
        protected readonly System.Type WrappedType;

        public MonoEquippable(object equippable)
        {
            Wrapped = equippable;
            WrappedType = equippable?.GetType();
        }

        public GameObject GameObject => WrappedType?.GetProperty("gameObject")?.GetValue(Wrapped) as GameObject;
        public string Path => WrappedType?.GetProperty("AssetPath")?.GetValue(Wrapped) as string;
    }
}
