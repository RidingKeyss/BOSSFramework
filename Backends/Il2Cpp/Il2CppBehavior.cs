// BOSSFramework - Backends/Il2Cpp/Il2CppBehavior.cs
// IL2CPP wrapper for Schedule I behaviors
using BOSSFramework.Shared;
using Il2CppPathfinding;
using Il2CppScheduleOne.NPCs.Behaviour;
using MelonLoader;

namespace BOSSFramework.Backends.Il2Cpp
{
    public class Il2CppBehavior : IBehavior
    {
        public readonly Behaviour Wrapped;

        public Il2CppBehavior(Behaviour wrapped)
        {
            if (wrapped == null)
                MelonLogger.Warning("[BOSSFramework] WARNING: Attempted to wrap null behavior!");
            Wrapped = wrapped;
        }

        public string Name
        {
            get => Wrapped.name;
            set => Wrapped.name = value;
        }

        public bool Active
        {
            get => Wrapped.Active;
            set => Wrapped.Active = value;
        }

        public void BehaviourUpdate() => Wrapped.BehaviourUpdate();

        public void EnableNetworked(object connection)
        {
            if (connection is Il2CppFishNet.Connection.NetworkConnection netConn)
                Wrapped.Enable_Networked(netConn);
        }

        public void BeginNetworked(object connection)
        {
            if (connection is Il2CppFishNet.Connection.NetworkConnection netConn)
                Wrapped.Begin_Networked(netConn);
        }

        public void EndNetworked(object connection)
        {
            if (connection is Il2CppFishNet.Connection.NetworkConnection netConn)
                Wrapped.End_Networked(netConn);
        }

        public static Il2CppBehavior? FromInterface(IBehavior? behavior)
        {
            return behavior as Il2CppBehavior;
        }

        public static Behaviour? Unwrap(IBehavior? behavior)
        {
            return (behavior as Il2CppBehavior)?.Wrapped;
        }
    }
}
