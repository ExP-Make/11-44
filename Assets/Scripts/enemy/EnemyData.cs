using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Data", menuName = "Enemy System/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Identification")]
    public string enemyName = "Basic Enemy";
    public bool isBoss = false;

    [Header("Core Stats")]
    public float maxHealth = 100f;
    public float moveSpeed = 3f;
    
    [Header("AI & Behavior")]
    public EnemyAIType aiType = EnemyAIType.Aggressive;
    public float detectionRange = 8f;
    public float attackRange = 2f;
    public float patrolDistance = 5f;
    public float alertTime = 3f;
    public bool canRetreat = false;
    public float retreatHealthThreshold = 0.3f;

    [Header("Movement")]
    public EnemyMovementType movementType = EnemyMovementType.Ground;
    public bool canJump = false;
    public float jumpForce = 5f;

    [Header("Flying Settings")]
    public float hoverHeight = 3f;
    public float maxFlyHeight = 8f;
    public float minFlyHeight = 1f;
    public float hoverAmplitude = 0.5f;
    public float hoverFrequency = 2f;
    
    [Header("Combat Patterns")]
    public ScriptableAttackPattern[] attackPatterns;
    public ScriptableSpecialPassive[] specialPassives;

    [Header("Visuals & Audio")]
    public Sprite idleSprite;
    public Sprite movingSprite;
    public Color damageFlashColor = Color.red;
    public AudioClip[] moveSounds;
    public AudioClip[] attackSounds;
    public AudioClip deathSound;
    public GameObject deathEffect;
    public GameObject spawnEffect;
    
    [Header("Drops & Rewards")]
    public DropItem[] dropItems;
    public int experienceValue = 10;
    
    [Header("Boss Specifics")]
    public GameObject bossUIPrefab;
    public AudioClip bossMusic;
}

[System.Serializable]
public class DropItem
{
    public GameObject itemPrefab;
    [Range(0, 1)] public float dropChance = 0.3f;
    public int minQuantity = 1;
    public int maxQuantity = 1;
} 