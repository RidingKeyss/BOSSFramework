﻿// BOSSFramework - Utils/Interfaces.cs
// Shared interfaces for dual IL2CPP and Mono compatibility
using ScheduleOne.Equipping;
using UnityEngine;

namespace BOSSCoreShared
{
    public interface INPC
    {
        GameObject GameObject { get; }
        Transform Transform { get; }
        string Name { get; }
        void PlayVoiceLine(VoiceLineType type);
        IDialogueRenderer GetDialogueRenderer();

        // Movement & Navigation
        bool CanMove();
        bool IsMoving { get; }
        void SetDestination(Vector3 position);
        void PauseMovement();
        void ResumeMovement();

        // Contextual Movement States
        bool IsInBuilding { get; }
        bool IsInVehicle { get; }
        void ExitBuilding();
        void ExitVehicle();

        // Pathfinding
        bool GetClosestReachablePoint(Vector3 targetPosition, out Vector3 result);

        // Behavior
        IBehavior ActiveBehavior { get; set; }
        List<IBehavior> EnabledBehaviors { get; }
        void AddEnabledBehavior(IBehavior behavior);
        void RemoveEnabledBehavior(IBehavior behavior);

        //Networking
        object LocalConnection { get; }

        //Avatar
        Transform AvatarRoot { get; }
        IEquippable SetEquippable(string id);
        IEquippable SetEquippableNetworked(object connection, string id);

        //Custom attributes
        float Accuracy { get; set; }
    }


    public interface IPlayer
    {
        GameObject GameObject { get; }
        Transform Transform { get; }
        Vector3 Position { get; }
        string DisplayName { get; }
    }

    public interface IDialogueRenderer
    {
        void ShowText(string text, float duration);
    }

    public interface IBehavior
    {
        string Name { get; set; }
        bool Active { get; set; }
        void BehaviourUpdate();
        void EnableNetworked(object connection);
        void BeginNetworked(object connection);
        void EndNetworked(object connection);
    }
    public interface IBehaviorCloner
    {
        IBehavior Clone(IBehavior template, string name);
    }

    // Shared enum to abstract backend-specific VO types
    public enum VoiceLineType
    {
        Acknowledge,
        Angry,
        Alerted,
        Greeting,
        // Add others as needed
    }

    public interface IIdleTemplateProvider
    {
        IBehavior? GetIdleTemplate();
    }
    public interface IEquippable
    {
        string Path { get; }
        GameObject GameObject { get; }
    }

    public interface IAvatarWeapon : IEquippable
    {
        void Attack();
    }

    public interface IAvatarMeleeWeapon : IAvatarWeapon
    {
        float AttackRadius { get; }
        float AttackRange { get; }
    }

    public interface IAvatarRangedWeapon : IAvatarWeapon
    {
        bool IsPlayerInLoS(IPlayer player);
        void SetIsRaised(bool isRaised);
        void Shoot(Vector3 endPoint);
    }

}
