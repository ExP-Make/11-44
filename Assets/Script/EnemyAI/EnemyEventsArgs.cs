using System;
using UnityEngine;

namespace EnemyAI
{
    public sealed class EnemyEventArgs : EventArgs
    {
        // Add Player Event, Args
        /public GameObject Player { get; }
        public PlayerEventArgs(GameObject player) => Player = player;

        public GameObject Enemy { get; }

        public EnemyEventArgs(GameObject enemy) => Enemy = enemy;
    }
}
