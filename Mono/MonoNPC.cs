// BOSSFramework - Backends/Mono/MonoNPC.cs
// Mono Implementation of INPC for Schedule I

using UnityEngine;
using ScheduleOne.NPCs;
using BOSSCoreShared;
using ScheduleOne.UI;
using System.Reflection;

namespace BOSSMono
{
    public class MonoNPC : INPC
    {
        private readonly NPC _npc;
        private float _accuracy = 0.5f;

        public MonoNPC(NPC npc)
        {
            _npc = npc;
        }

        public GameObject GameObject => _npc.gameObject;
        public Transform Transform => _npc.transform;
        public string Name => _npc.name;
        public Transform AvatarRoot => _npc.Avatar?.BodyContainer?.transform;

        public void PlayVoiceLine(VoiceLineType type)
        {
            // Voice lines would need Mono-compatible VO mapping logic
        }

        public bool CanMove() => _npc.Movement.CanMove();
        public bool IsMoving => _npc.Movement.IsMoving;
        public void SetDestination(Vector3 position) => _npc.Movement.SetDestination(position);
        public void PauseMovement() => _npc.Movement.PauseMovement();
        public void ResumeMovement() => _npc.Movement.ResumeMovement();

        public bool IsInBuilding => _npc.isInBuilding;
        public bool IsInVehicle => _npc.IsInVehicle;
        public void ExitBuilding() => _npc.ExitBuilding();
        public void ExitVehicle() => _npc.ExitVehicle();

        public bool GetClosestReachablePoint(Vector3 targetPosition, out Vector3 result)
            => _npc.Movement.GetClosestReachablePoint(targetPosition, out result);

        public IDialogueRenderer GetDialogueRenderer()
        {
            var spine2 = BOSSUtils.FindChildRecursive(AvatarRoot, "mixamorig:Spine2");
            if (spine2 == null) return null;

            var renderer = spine2.GetComponentInChildren<WorldspaceDialogueRenderer>();
            return renderer != null ? new MonoDialogueRenderer(renderer) : null;
        }

        public IBehavior ActiveBehavior
        {
            get => new MonoBehavior(_npc.behaviour.activeBehaviour);
            set => _npc.behaviour.activeBehaviour = MonoBehavior.Unwrap(value);
        }

        public List<IBehavior> EnabledBehaviors
        {
            get
            {
                var result = new List<IBehavior>();
                var stack = MonoReflectionUtils.GetBehaviourStack(_npc.behaviour, "enabledBehaviours");
                if (stack == null) return result;

                foreach (var b in stack)
                {
                    if (b != null)
                        result.Add(new MonoBehavior(b));
                }
                return result;
            }
        }

        public void AddEnabledBehavior(IBehavior behavior)
        {
            if (behavior is MonoBehavior mono)
            {
                var method = _npc.behaviour.GetType().GetMethod("AddEnabledBehaviour", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                method?.Invoke(_npc.behaviour, new object[] { MonoBehavior.Unwrap(behavior) });
            }
        }

        public void RemoveEnabledBehavior(IBehavior behavior)
        {
            if (behavior is MonoBehavior mono)
            {
                var method = _npc.behaviour.GetType().GetMethod("RemoveEnabledBehaviour", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                method?.Invoke(_npc.behaviour, new object[] { MonoBehavior.Unwrap(behavior) });
            }
        }

        public object LocalConnection => _npc.LocalConnection;

        public IEquippable SetEquippable(string id)
        {
            var method = _npc.Avatar.GetType().GetMethod("SetEquippable_Return", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var result = method?.Invoke(_npc.Avatar, new object[] { id });
            return new MonoEquippable(result);
        }

        public IEquippable SetEquippableNetworked(object connection, string id)
        {
            var method = _npc.Avatar.GetType().GetMethod("SetEquippable_Networked_Return", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var result = method?.Invoke(_npc.Avatar, new object[] { connection, id });
            return new MonoEquippable(result);
        }

        public float Accuracy
        {
            get => _accuracy;
            set => _accuracy = Mathf.Clamp01(value);
        }
    }
}
