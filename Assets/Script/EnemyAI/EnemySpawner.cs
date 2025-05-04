using System;
using UnityEngine;

namespace EnemyAI
{
    public sealed class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private EnemyAI enemyPrefab;
        [SerializeField] private Transform[] spawnPoints;
        public event EventHandler<EnemyEventArgs> OnEnemySpawned;

        private void Start()
        {
            if (enemyPrefab == null || spawnPoints == null || spawnPoints.Length == 0)
            {
                Debug.LogWarning($"{name}: EnemyPrefab or SpawnPoints not set.");
                return;
            }

            foreach (var point in spawnPoints)
            {
                EnemyAI enemy = Instantiate(enemyPrefab, point.position, point.rotation);
                OnEnemySpawned?.Invoke(this, new EnemyEventArgs(enemy.gameObject));
            }
        }
    }
}
