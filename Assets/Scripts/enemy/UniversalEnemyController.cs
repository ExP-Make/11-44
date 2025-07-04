using UnityEngine;
using UnityEngine.Events;

public class UniversalEnemyController : MonoBehaviour
{
    [Header("Enemy Configuration")]
    public EnemyData enemyData;
    
    [Header("Scriptable Components")]
    public ScriptableAttackPattern[] scriptableAttackPatterns;
    public ScriptableSpecialPassive[] scriptableSpecialPassives;
    
    // Public properties for external access, hidden from inspector
    public UniversalEnemyAI EnemyAI { get; private set; }
    public UniversalEnemyHealth EnemyHealth { get; private set; }
    
    [Header("Private References")]
    private Rigidbody2D rb;
    private Collider2D enemyCollider;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private AudioSource audioSource;
    
    [Header("Layer Setup")]
    public LayerMask playerLayer = 1 << 8;
    public LayerMask obstacleLayer = 1 << 9;
    public LayerMask groundLayer = 1 << 10;
    
    [Header("Events")]
    public UnityEvent<UniversalEnemyController> OnEnemySpawned = new UnityEvent<UniversalEnemyController>();
    public UnityEvent<UniversalEnemyController> OnEnemyDeath = new UnityEvent<UniversalEnemyController>();
    public UnityEvent<UniversalEnemyController, float> OnEnemyDamaged = new UnityEvent<UniversalEnemyController, float>();
    public UnityEvent<UniversalEnemyController> OnEnemyPlayerDetected = new UnityEvent<UniversalEnemyController>();
    
    [Header("Runtime References")]
    public GameObject projectilePrefab;
    
    private bool isInitialized = false;
    private float initializationTime;
    
    void Awake()
    {
        if (enemyData == null)
        {
            Debug.LogError("Enemy Data is not assigned on " + gameObject.name, this);
            gameObject.SetActive(false);
            return;
        }

        SetupComponents();
        InitializeEnemy();
    }
    
    private void SetupComponents()
    {
        rb = gameObject.GetOrAdd<Rigidbody2D>();
        audioSource = gameObject.GetOrAdd<AudioSource>();
        spriteRenderer = gameObject.GetOrAdd<SpriteRenderer>();
        EnemyAI = gameObject.GetOrAdd<UniversalEnemyAI>();
        EnemyHealth = gameObject.GetOrAdd<UniversalEnemyHealth>();
        animator = GetComponent<Animator>(); 

        enemyCollider = GetComponent<Collider2D>();
        if (enemyCollider == null)
        {
            enemyCollider = gameObject.AddComponent<BoxCollider2D>();
            enemyCollider.isTrigger = true;
        }

        if (scriptableAttackPatterns == null || scriptableAttackPatterns.Length == 0)
        {
            scriptableAttackPatterns = GetComponents<ScriptableAttackPattern>();
        }
        if (scriptableSpecialPassives == null || scriptableSpecialPassives.Length == 0)
        {
            scriptableSpecialPassives = GetComponents<ScriptableSpecialPassive>();
        }
    }
    
    void InitializeEnemy()
    {
        if (isInitialized) return;
        
        gameObject.name = enemyData.enemyName;
        
        SetupPhysics();
        SetupVisuals();
        SetupAudio();
        
        EnemyAI.Initialize(enemyData, this);
        EnemyHealth.Initialize(enemyData, this);
        
        SubscribeToEvents();
        
        if (enemyData.spawnEffect != null)
        {
            Instantiate(enemyData.spawnEffect, transform.position, transform.rotation);
        }
        
        isInitialized = true;
        initializationTime = Time.time;
        
        OnEnemySpawned?.Invoke(this);
        
        if (enemyData.isBoss) 
        {
            BossBattleManager.Instance.StartBoss(this);
        }
    }
    
    void SetupPhysics()
    {
        rb.gravityScale = enemyData.movementType == EnemyMovementType.Flying ? 0f : 1f;
        rb.linearDamping = enemyData.movementType == EnemyMovementType.Flying ? 2f : 0f;
        rb.freezeRotation = true;
    }
    
    void SetupVisuals()
    {
        if (enemyData.idleSprite != null)
        {
            spriteRenderer.sprite = enemyData.idleSprite;
        }
    }
    
    void SetupAudio()
    {
        audioSource.playOnAwake = false;
        audioSource.volume = 0.7f;
    }
    
    void SubscribeToEvents()
    {
        EnemyHealth.OnDamageTaken.AddListener(HandleDamageTaken);
        EnemyHealth.OnHealthChanged.AddListener(HandleHealthChanged);
        EnemyHealth.OnDeath.AddListener(HandleDeath);
        
        if (EnemyAI != null)
        {
            EnemyAI.OnPlayerDetected.AddListener(HandlePlayerDetected);
        }
    }
    
    void HandleDamageTaken(float damage)
    {
        OnEnemyDamaged?.Invoke(this, damage);
        PlaySoundFromData("damage");
    }
    
    void HandleHealthChanged(float newHealth, float maxHealth)
    {
        
    }
    
    void HandleDeath()
    {
        OnEnemyDeath?.Invoke(this);
        PlaySoundFromData("death");
        
        if (enemyData != null && enemyData.isBoss)
        {
            BossBattleManager.Instance.EndBoss();
        }
    }
    
    void HandlePlayerDetected()
    {
        OnEnemyPlayerDetected?.Invoke(this);
    }
    
    public void SetEnemyData(EnemyData newEnemyData)
    {
        enemyData = newEnemyData;
        
        if (isInitialized)
        {
            EnemyAI.UpdateEnemyData(newEnemyData);
            EnemyHealth.UpdateEnemyData(newEnemyData);
        }
        else
        {
            InitializeEnemy();
        }
    }
    
    public void TakeDamage(float damage, Vector2 damageDirection = default)
    {
        if (EnemyHealth != null)
        {
            EnemyHealth.TakeDamage(damage, damageDirection);
        }
    }
    
    public void Heal(float healAmount)
    {
        if (EnemyHealth != null)
        {
            EnemyHealth.Heal(healAmount);
        }
    }
    
    public void ApplyKnockback(Vector2 knockbackDirection, float knockbackForce)
    {
        if (rb != null)
        {
            rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
        }
    }
    
    public void ForceState(string stateName)
    {
        if (EnemyAI != null)
        {
            EnemyAI.ForceState(stateName);
        }
    }
    
    public void AddSpecialPassive(ScriptableSpecialPassive passive)
    {
        if (EnemyAI != null)
        {
            EnemyAI.AddSpecialPassive(passive);
        }
    }
    
    public void RemoveSpecialPassive(ScriptableSpecialPassive passive)
    {
        if (EnemyAI != null)
        {
            EnemyAI.RemoveSpecialPassive(passive);
        }
    }
    
    public void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    
    public void PlaySoundFromData(string soundType)
    {
        if (enemyData == null || audioSource == null) return;
        
        AudioClip clipToPlay = null;
        
        switch (soundType.ToLower())
        {
            case "move":
                if (enemyData.moveSounds != null && enemyData.moveSounds.Length > 0)
                {
                    clipToPlay = enemyData.moveSounds[Random.Range(0, enemyData.moveSounds.Length)];
                }
                break;
            case "attack":
                if (enemyData.attackSounds != null && enemyData.attackSounds.Length > 0)
                {
                    clipToPlay = enemyData.attackSounds[Random.Range(0, enemyData.attackSounds.Length)];
                }
                break;
            case "death": 
                clipToPlay = enemyData.deathSound; 
                break;
            case "damage": 
                break;
        }
        
        if (clipToPlay != null)
        {
            audioSource.PlayOneShot(clipToPlay);
        }
    }
    
    public void SpawnProjectile(GameObject projectilePrefab, Vector2 direction, float speed)
    {
        if (projectilePrefab != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            
            Rigidbody2D projRb = projectile.GetComponent<Rigidbody2D>();
            if (projRb != null)
            {
                projRb.linearVelocity = direction.normalized * speed;
            }
            
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            projectile.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
    
    public void SpawnEffect(GameObject effectPrefab, Vector3 position, Quaternion rotation)
    {
        if (effectPrefab != null)
        {
            Instantiate(effectPrefab, position, rotation);
        }
    }
    
    public void DestroyEnemy(float delay = 0f)
    {
        if (delay > 0f)
        {
            Destroy(gameObject, delay);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public float GetCurrentHealth()
    {
        return EnemyHealth != null ? EnemyHealth.GetCurrentHealth() : 0f;
    }
    
    public float GetMaxHealth()
    {
        return enemyData != null ? enemyData.maxHealth : 0f;
    }
    
    public float GetHealthPercentage()
    {
        return EnemyHealth != null ? EnemyHealth.GetHealthPercentage() : 0f;
    }
    
    public bool IsAlive()
    {
        return EnemyHealth != null && EnemyHealth.IsAlive();
    }
    
    public Vector2 GetVelocity()
    {
        return rb != null ? rb.linearVelocity : Vector2.zero;
    }
    
    public float GetDistanceToPlayer()
    {
        return EnemyAI != null ? EnemyAI.GetDistanceToPlayer() : float.MaxValue;
    }
    
    public bool HasLineOfSightToPlayer()
    {
        return EnemyAI != null && EnemyAI.HasLineOfSightToPlayer();
    }
    
    public string GetCurrentState()
    {
        return EnemyAI != null ? EnemyAI.GetCurrentState() : "Unknown";
    }
    
    public bool CanAttack()
    {
        return EnemyAI != null && EnemyAI.CanAttack();
    }
    
    public void SetPatrolArea(Vector2 center, float radius)
    {
        if (EnemyAI != null)
        {
            EnemyAI.SetPatrolArea(center, radius);
        }
    }
    
    public void SetTarget(Transform target)
    {
        if (EnemyAI != null)
        {
            EnemyAI.SetTarget(target);
        }
    }
    
    // Layer mask properties for scriptable components
    public LayerMask PlayerLayer => playerLayer;
    public LayerMask ObstacleLayer => obstacleLayer;
    public LayerMask GroundLayer => groundLayer;
    
    public static UniversalEnemyController CreateEnemy(EnemyData enemyData, Vector3 position, Quaternion rotation)
    {
        GameObject enemyObject = new GameObject(enemyData.enemyName);
        enemyObject.transform.position = position;
        enemyObject.transform.rotation = rotation;
        
        UniversalEnemyController controller = enemyObject.AddComponent<UniversalEnemyController>();
        controller.SetEnemyData(enemyData);
        
        return controller;
    }
    
    public static UniversalEnemyController CreateEnemy(EnemyData enemyData, Vector3 position)
    {
        return CreateEnemy(enemyData, position, Quaternion.identity);
    }
    
    void OnDrawGizmosSelected()
    {
        if (enemyData != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, enemyData.detectionRange);
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, enemyData.attackRange);
            
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, enemyData.patrolDistance);
        }
    }

    public float GetOriginalSpeed()
    {
        return enemyData != null ? enemyData.moveSpeed : 0f;
    }

    public void SetSpeed(float newSpeed)
    {
        if(enemyData != null)
        {
            enemyData.moveSpeed = newSpeed;
        }
    }

    public void Move(Vector2 direction)
    {
        if (rb != null)
        {
            rb.linearVelocity = direction * (enemyData != null ? enemyData.moveSpeed : 0f);
        }
    }
} 