// BOSSFramework - Mono/MonoPlayer.cs
// Mono implementation of IPlayer

using UnityEngine;
using ScheduleOne.PlayerScripts;
using BOSSCoreShared;

namespace BOSSMono
{
    public class MonoPlayer : IPlayer
    {
        private readonly Player _player;

        public MonoPlayer(Player player)
        {
            _player = player;
        }

        public GameObject GameObject => _player.gameObject;
        public Transform Transform => _player.transform;
        public Vector3 Position => _player.transform.position;
        public string DisplayName => _player.name;
    }
}
