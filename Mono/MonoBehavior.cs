// BOSSFramework - MonoBehavior.cs
// Mono wrapper for IBehavior

using BOSSCoreShared;
using ScheduleOne.NPCs.Behaviour;
using FishNet.Connection;
using System.Reflection;

namespace BOSSMono
{
    public class MonoBehavior : IBehavior
    {
        public Behaviour Wrapped { get; private set; }

        public MonoBehavior(Behaviour wrapped)
        {
            if (wrapped == null)
                MelonLoader.MelonLogger.Warning("[BOSSFramework] [Mono] WARNING: Attempted to wrap null behavior!");
            Wrapped = wrapped;
        }

        public bool Active
        {
            get => Wrapped.Active;
            set => SetActive(value);
        }

        private void SetActive(bool value)
        {
            var prop = Wrapped.GetType().GetProperty("Active", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (prop?.CanWrite == true)
            {
                prop.SetValue(Wrapped, value);
            }
            else
            {
                MelonLoader.MelonLogger.Warning("[BOSSFramework] [Mono] Cannot set 'Active' — no setter available.");
            }
        }

        public string Name
        {
            get => Wrapped.name;
            set => Wrapped.name = value;
        }

        public void BehaviourUpdate() => Wrapped.BehaviourUpdate();

        public void EnableNetworked(object connection)
        {
            if (connection is NetworkConnection netConn)
                Wrapped.Enable_Networked(netConn);
        }

        public void BeginNetworked(object connection)
        {
            if (connection is NetworkConnection netConn)
                Wrapped.Begin_Networked(netConn);
        }

        public void EndNetworked(object connection)
        {
            if (connection is NetworkConnection netConn)
                Wrapped.End_Networked(netConn);
        }

        public static MonoBehavior? FromInterface(IBehavior behavior)
        {
            return behavior as MonoBehavior;
        }

        public static Behaviour? Unwrap(IBehavior? behavior) => (behavior as MonoBehavior)?.Wrapped;
    }
}
