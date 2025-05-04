using UnityEngine;

namespace EnemyAI
{
    [CreateAssetMenu(menuName = "Prefab/Enemy/Stats", fileName = "EnemyStats")]
    public sealed class EnemyStats : ScriptableObject
    {
        [Min(1)] public int maxHp = 100;
        public bool isFlying = false;
        [Min(0.1f)] public float moveSpeed = 3f;
    }
}