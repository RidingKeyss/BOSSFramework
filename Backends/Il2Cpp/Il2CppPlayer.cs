// IL2CPP wrapper for Schedule I player objects
using UnityEngine;
using BOSSFramework.Shared;
using Il2CppScheduleOne.PlayerScripts;

namespace BOSSFramework.Backends.Il2Cpp
{
    public class Il2CppPlayer : IPlayer
    {
        private readonly Player _player;

        public Il2CppPlayer(Player player)
        {
            _player = player;
        }

        public GameObject GameObject => _player.gameObject;
        public Transform Transform => _player.transform;
        public Vector3 Position => _player.transform.position;
        public string DisplayName => _player.name;
    }
}
