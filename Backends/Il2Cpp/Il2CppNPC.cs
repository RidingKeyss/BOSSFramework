// BOSSFramework - Backends/Il2Cpp/Il2CppNPC.cs
// IL2CPP Implementation of INPC for Schedule I
using UnityEngine;
using Il2CppScheduleOne.NPCs;
using Il2CppScheduleOne.VoiceOver;
using BOSSFramework.Shared;

namespace BOSSFramework.Backends.Il2Cpp
{
    public class Il2CppNPC : INPC
    {
        private readonly NPC _npc;

        public Il2CppNPC(NPC npc)
        {
            _npc = npc;
        }

        public GameObject GameObject => _npc.gameObject;
        public Transform Transform => _npc.transform;
        public string Name => _npc.name;
        public Transform AvatarRoot => _npc.Avatar?.BodyContainer?.transform;

        public void PlayVoiceLine(VoiceLineType type)
        {
            var mapped = type switch
            {
                VoiceLineType.Acknowledge => EVOLineType.Acknowledge,
                VoiceLineType.Angry => EVOLineType.Angry,
                VoiceLineType.Alerted => EVOLineType.Alerted,
                VoiceLineType.Greeting => EVOLineType.Greeting,
                _ => EVOLineType.Acknowledge
            };

            _npc.PlayVO(mapped);
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

            var renderer = spine2.GetComponentInChildren<Il2CppScheduleOne.UI.WorldspaceDialogueRenderer>();
            return renderer != null ? new Il2CppDialogueRenderer(renderer) : null;
        }

        public IBehavior? ActiveBehavior
        {
            get => _npc.behaviour.activeBehaviour as IBehavior;
            set => _npc.behaviour.activeBehaviour = Il2CppBehavior.Unwrap(value);
        }

        public List<IBehavior> EnabledBehaviors
        {
            get
            {
                var result = new List<IBehavior>();
                foreach (var b in _npc.behaviour.behaviourStack)
                {
                    if (b != null)
                        result.Add(new Il2CppBehavior(b));
                }
                return result;
            }
        }

        public void AddEnabledBehavior(IBehavior behavior)
        {
            if (behavior is Il2CppBehavior il2cpp)
                _npc.behaviour.AddEnabledBehaviour(Il2CppBehavior.Unwrap(behavior));
        }

        public void RemoveEnabledBehavior(IBehavior behavior)
        {
            if (behavior is Il2CppBehavior il2cpp)
                _npc.behaviour.RemoveEnabledBehaviour(Il2CppBehavior.Unwrap(behavior));
        }

        public object? LocalConnection => _npc.LocalConnection;
    }
}
