using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum AreaEffectType
{
    Damage,
    Heal
}

public class UniversalEnemyAI : MonoBehaviour
{
    [Header("Configuration")]
    public EnemyData enemyData;
    
    [Header("Runtime References")]
    public Transform player;
    public Rigidbody2D rb;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public AudioSource audioSource;
    
    [Header("Layer Masks")]
    public LayerMask playerLayer;
    public LayerMask obstacleLayer;
    public LayerMask groundLayer;
    
    [Header("Events")]
    public UnityEvent OnPlayerDetected = new UnityEvent();
    
    private UniversalEnemyController controller;
    private UniversalEnemyState currentState;
    private Vector3 startPosition;
    private Vector3 patrolTarget;
    private float lastAttackTime;
    private float alertTimer;
    private bool hasSeenPlayer;
    private bool wasRecentlyDamaged;
    private float lastDamageTime;
    
    private Dictionary<ScriptableAttackPattern, float> attackCooldowns;
    private List<ScriptableSpecialPassive> activePassives;
    private Color originalColor;
    private float originalSpeed;
    private float originalDamage;
    private float originalDefense;
    
    private bool isInvulnerable;
    private bool isRegenerating;
    private bool isPhasing;
    private Vector3 hoverOffset;
    
    public enum UniversalEnemyState
    {
        Spawning,
        Idle,
        Patrol,
        Alert,
        Chase,
        Attack,
        Retreat,
        Death,
        Special
    }
    
    private bool isInitialized = false;

    public void Initialize(EnemyData data, UniversalEnemyController owner)
    {
        this.enemyData = data;
        this.controller = owner;
        this.rb = owner.GetComponent<Rigidbody2D>();
        this.playerLayer = owner.PlayerLayer;
        this.obstacleLayer = owner.ObstacleLayer;
        this.groundLayer = owner.GroundLayer;
        
        if (!isInitialized)
        {
            InitializeEnemy();
            SetupComponents();
            ActivatePassives();
            isInitialized = true;
        }
    }
    
    void InitializeEnemy()
    {
        if (enemyData == null)
        {
            Debug.LogError("EnemyData is not assigned!", this);
            return;
        }
        
        originalSpeed = enemyData.moveSpeed;
        originalDamage = 1f;
        originalDefense = 1f;
        startPosition = transform.position;
        
        attackCooldowns = new Dictionary<ScriptableAttackPattern, float>();
        activePassives = new List<ScriptableSpecialPassive>();
        
        if (enemyData.attackPatterns != null)
        {
            foreach (var pattern in enemyData.attackPatterns)
            {
                attackCooldowns[pattern] = 0f;
            }
        }
        
        currentState = UniversalEnemyState.Spawning;
        SetNewPatrolTarget();
        
        if (enemyData.movementType == EnemyMovementType.Flying)
        {
            SetupFlyingMovement();
        }
    }
    
    void SetupComponents()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
        
        if (enemyData.specialPassives != null)
        {
            foreach (var passive in enemyData.specialPassives)
            {
                if (passive != null && passive.Trigger == PassiveTrigger.OnSpawn)
                {
                    activePassives.Add(passive);
                    if (controller != null)
                    {
                        passive.Activate(controller);
                    }
                }
            }
        }
    }
    
    void SetupFlyingMovement()
    {
        rb.gravityScale = 0f;
        rb.linearDamping = 2f;
        
        hoverOffset = new Vector3(0, Random.Range(0f, Mathf.PI * 2f), 0);
    }
    
    void ActivatePassives()
    {
        if (enemyData.specialPassives != null)
        {
            foreach (var passive in enemyData.specialPassives)
            {
                if (passive != null && passive.Trigger == PassiveTrigger.OnSpawn)
                {
                    activePassives.Add(passive);
                    if (controller != null)
                    {
                        passive.Activate(controller);
                    }
                }
            }
        }
    }
    
    void Update()
    {
        if (enemyData == null) return;
        
        UpdateCooldowns();
        UpdateState();
        HandleMovement();
        HandleAttacks();
        CheckPassiveTriggers();
        UpdateScriptablePassives();
        UpdateAnimations();
        
        wasRecentlyDamaged = Time.time - lastDamageTime < 0.5f;
    }
    
    void UpdateScriptablePassives()
    {
        if (controller != null && controller.scriptableSpecialPassives != null)
        {
            foreach (var scriptablePassive in controller.scriptableSpecialPassives)
            {
                if (scriptablePassive != null)
                {
                    scriptablePassive.PassiveUpdate(controller);
                }
            }
        }
    }
    
    void UpdateCooldowns()
    {
        foreach (var key in new List<ScriptableAttackPattern>(attackCooldowns.Keys))
        {
            if (attackCooldowns[key] <= 0f) continue;
            attackCooldowns[key] -= Time.deltaTime;
        }
    }
    
    void UpdateState()
    {
        if (ShouldRetreat())
        {
            currentState = UniversalEnemyState.Retreat;
        }

        switch(currentState)
        {
            case UniversalEnemyState.Spawning:
                HandleSpawningState();
                break;
                
            case UniversalEnemyState.Idle:
            case UniversalEnemyState.Patrol:
                HandleIdlePatrolState();
                break;
                
            case UniversalEnemyState.Alert:
                HandleAlertState();
                break;
                
            case UniversalEnemyState.Chase:
                HandleChaseState();
                break;
                
            case UniversalEnemyState.Attack:
                HandleAttackState();
                break;
                
            case UniversalEnemyState.Retreat:
                HandleRetreatState();
                break;
        }
    }
    
    #region State Handlers
    private void HandleSpawningState()
    {
        if (Time.time > 1f)
            currentState = GetInitialState();
    }

    private void HandleIdlePatrolState()
    {
        CheckPlayerVisibility();
        if (hasSeenPlayer && enemyData.aiType != EnemyAIType.Passive)
        {
            currentState = UniversalEnemyState.Alert;
            alertTimer = enemyData.alertTime;
        }
    }

    private void HandleAlertState()
    {
        alertTimer -= Time.deltaTime;
        CheckPlayerVisibility();
        if (hasSeenPlayer)
        {
            currentState = GetChaseState();
        }
        else if (alertTimer <= 0)
        {
            currentState = GetInitialState();
        }
    }

    private void HandleChaseState()
    {
        float distanceToPlayer = GetDistanceToPlayer();
        if (distanceToPlayer <= enemyData.attackRange)
        {
            currentState = UniversalEnemyState.Attack;
        }
        else
        {
            CheckPlayerVisibility();
            if (!hasSeenPlayer)
            {
                currentState = UniversalEnemyState.Alert;
                alertTimer = enemyData.alertTime;
            }
        }
    }

    private void HandleAttackState()
    {
        if (GetDistanceToPlayer() > enemyData.attackRange)
        {
            currentState = UniversalEnemyState.Chase;
        }
    }

    private void HandleRetreatState()
    {
        if (GetHealthPercentage() > enemyData.retreatHealthThreshold)
        {
            currentState = UniversalEnemyState.Chase;
        }
    }

    private bool ShouldRetreat()
    {
        return enemyData.canRetreat && GetHealthPercentage() <= enemyData.retreatHealthThreshold;
    }
    #endregion
    
    UniversalEnemyState GetInitialState()
    {
        switch (enemyData.aiType)
        {
            case EnemyAIType.Passive:
                return UniversalEnemyState.Idle;
            case EnemyAIType.Territorial:
                return UniversalEnemyState.Patrol;
            default:
                return UniversalEnemyState.Patrol;
        }
    }
    
    UniversalEnemyState GetChaseState()
    {
        switch (enemyData.aiType)
        {
            case EnemyAIType.Defensive:
                return UniversalEnemyState.Alert;
            default:
                return UniversalEnemyState.Chase;
        }
    }
    
    void HandleMovement()
    {
        switch (enemyData.movementType)
        {
            case EnemyMovementType.Ground:
                HandleGroundMovement();
                break;
            case EnemyMovementType.Flying:
                HandleFlyingMovement();
                break;
            case EnemyMovementType.Hybrid:
                HandleHybridMovement();
                break;
        }
    }
    
    void HandleGroundMovement()
    {
        Vector2 targetVelocity = Vector2.zero;
        
        switch (currentState)
        {
            case UniversalEnemyState.Patrol:
                targetVelocity = GetPatrolMovement();
                break;
            case UniversalEnemyState.Chase:
                targetVelocity = GetChaseMovement();
                break;
            case UniversalEnemyState.Retreat:
                targetVelocity = GetRetreatMovement();
                break;
        }
        
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(targetVelocity.x, rb.linearVelocity.y);
        }
    }
    
    void HandleFlyingMovement()
    {
        Vector2 targetPosition = transform.position;
        bool shouldMove = true;
        
        switch (currentState)
        {
            case UniversalEnemyState.Patrol:
                targetPosition = GetFlyingPatrolTarget();
                break;
            case UniversalEnemyState.Chase:
                targetPosition = GetFlyingChaseTarget();
                if (GetDistanceToPlayer() <= enemyData.attackRange)
                {
                    shouldMove = false;
                }
                break;
            case UniversalEnemyState.Retreat:
                targetPosition = GetFlyingRetreatTarget();
                break;
            default:
                shouldMove = false;
                break;
        }
        
        ApplyHoverEffect();
        if (shouldMove)
        {
            MoveTo(targetPosition);
        }
        else
        {
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
    }
    
    void HandleHybridMovement()
    {
        if (GetDistanceToPlayer() > enemyData.detectionRange * 0.5f)
        {
            HandleGroundMovement();
        }
        else
        {
            HandleFlyingMovement();
        }
    }
    
    Vector2 GetPatrolMovement()
    {
        Vector2 direction = (patrolTarget - transform.position).normalized;
        
        if (Vector2.Distance(transform.position, patrolTarget) < 0.5f)
        {
            SetNewPatrolTarget();
        }
        
        return direction * enemyData.moveSpeed;
    }
    
    Vector2 GetChaseMovement()
    {
        if (player == null) return Vector2.zero;
        
        Vector2 direction = (player.position - transform.position).normalized;
        return direction * enemyData.moveSpeed;
    }
    
    Vector2 GetRetreatMovement()
    {
        if (player == null) return Vector2.zero;
        
        Vector2 direction = (transform.position - player.position).normalized;
        return direction * enemyData.moveSpeed * 1.2f;
    }
    
    Vector2 GetFlyingPatrolTarget()
    {
        Vector2 target = patrolTarget;
        target.y = Mathf.Clamp(target.y, enemyData.minFlyHeight, enemyData.maxFlyHeight);
        
        if (Vector2.Distance(transform.position, target) < 1f)
        {
            SetNewPatrolTarget();
        }
        
        return target;
    }
    
    Vector2 GetFlyingChaseTarget()
    {
        if (player == null) return transform.position;
        
        return player.position;
    }
    
    Vector2 GetFlyingRetreatTarget()
    {
        Vector2 retreatDirection = (transform.position - player.position).normalized;
        Vector2 target = (Vector2)transform.position + retreatDirection * 5f;
        target.y = enemyData.maxFlyHeight;
        return target;
    }
    
    void ApplyHoverEffect()
    {
        float hoverY = Mathf.Sin(Time.time * enemyData.hoverFrequency + hoverOffset.y) * enemyData.hoverAmplitude;
        transform.position += Vector3.up * hoverY * Time.deltaTime;
    }
    
    void MoveTo(Vector2 target)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        if (rb != null)
        {
            rb.linearVelocity = direction * enemyData.moveSpeed;
        }
    }
    
    void SetNewPatrolTarget()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        patrolTarget = startPosition + (Vector3)randomDirection * enemyData.patrolDistance;
        
        if (enemyData.movementType == EnemyMovementType.Flying)
        {
            patrolTarget.y = Random.Range(enemyData.minFlyHeight, enemyData.maxFlyHeight);
        }
    }
    
    void HandleAttacks()
    {
        if (currentState != UniversalEnemyState.Attack)
            return;
            
        if (controller == null || controller.scriptableAttackPatterns == null) return;

        foreach (var scriptableAttack in controller.scriptableAttackPatterns)
        {
            if (scriptableAttack == null) continue;

            // Register pattern if not tracked yet
            if (!attackCooldowns.ContainsKey(scriptableAttack))
                attackCooldowns[scriptableAttack] = 0f;

            // Cooldown not ready
            if (attackCooldowns[scriptableAttack] > 0f) continue;

            if (!scriptableAttack.CanExecute(controller, player)) continue;

            // Execute and set cooldown
            StartCoroutine(ExecuteScriptableAttack(scriptableAttack, controller));
            attackCooldowns[scriptableAttack] = scriptableAttack.Cooldown;
            break; // 하나만 실행
        }
    }
    
    IEnumerator ExecuteScriptableAttack(IScriptableAttackPattern scriptableAttack, UniversalEnemyController controller)
    {
        lastAttackTime = Time.time;
        yield return StartCoroutine(scriptableAttack.ExecuteAttack(controller, player));
    }
    
    void CheckPassiveTriggers()
    {
        if (controller != null && controller.scriptableSpecialPassives != null)
        {
            foreach (var scriptablePassive in controller.scriptableSpecialPassives)
            {
                if (scriptablePassive != null && scriptablePassive.ShouldActivate(controller))
                {
                    scriptablePassive.Activate(controller);
                }
            }
        }
    }
    
    void CheckPlayerVisibility()
    {
        if (player == null) return;
        
        float distanceToPlayer = GetDistanceToPlayer();
        
        if (distanceToPlayer > enemyData.detectionRange)
        {
            hasSeenPlayer = false;
            return;
        }
        
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleLayer);
        
        bool canSeePlayer = hit.collider == null;
        
        if (canSeePlayer && !hasSeenPlayer)
        {
            OnPlayerDetected?.Invoke();
        }
        
        hasSeenPlayer = canSeePlayer;
    }
    
    public bool HasLineOfSightToPlayer()
    {
        if (player == null) return false;
        
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = GetDistanceToPlayer();
        
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleLayer);
        return hit.collider == null;
    }
    
    void UpdateAnimations()
    {
        if (animator == null) return;
        
        animator.SetFloat("Speed", rb != null ? rb.linearVelocity.magnitude : 0f);
        animator.SetBool("IsChasing", currentState == UniversalEnemyState.Chase);
        animator.SetBool("IsAttacking", currentState == UniversalEnemyState.Attack);
        animator.SetBool("IsDead", currentState == UniversalEnemyState.Death);
        
        if (enemyData.movementType == EnemyMovementType.Flying)
        {
            animator.SetBool("IsHovering", currentState == UniversalEnemyState.Idle || currentState == UniversalEnemyState.Patrol);
        }
    }
    
    IEnumerator DamageFlashEffect()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = enemyData.damageFlashColor;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
        }
    }
    
    public float GetDistanceToPlayer()
    {
        return player != null ? Vector2.Distance(transform.position, player.position) : float.MaxValue;
    }
    
    public float GetHealthPercentage()
    {
        return controller.GetHealthPercentage();
    }
    
    public bool WasRecentlyDamaged()
    {
        return wasRecentlyDamaged;
    }
    
    public void ModifyStats(float healthMod, float speedMod, float damageMod, float defenseMod)
    {
        enemyData.maxHealth *= healthMod;
        originalSpeed *= speedMod;
        originalDamage *= damageMod;
        originalDefense *= defenseMod;
    }
    
    public void ShowGlowEffect(Color glowColor)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.Lerp(originalColor, glowColor, 0.5f);
        }
    }
    
    public void StartRegeneration(float rate)
    {
        if (!isRegenerating)
        {
            StartCoroutine(RegenerationCoroutine(rate));
        }
    }
    
    IEnumerator RegenerationCoroutine(float rate)
    {
        isRegenerating = true;
        
        while (controller.GetHealthPercentage() < enemyData.maxHealth && currentState != UniversalEnemyState.Death)
        {
            controller.Heal(rate * Time.deltaTime);
            yield return null;
        }
        
        isRegenerating = false;
    }
    
    public void SpawnMinions(GameObject minionPrefab, int count, float radius)
    {
        for (int i = 0; i < count; i++)
        {
            Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * radius;
            Instantiate(minionPrefab, spawnPos, Quaternion.identity);
        }
    }
    
    public void ActivateAreaEffect(float radius, AreaEffectType effectType, float value)
    {
        StartCoroutine(AreaEffectCoroutine(radius, effectType, value));
    }
    
    IEnumerator AreaEffectCoroutine(float radius, AreaEffectType effectType, float value)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, playerLayer);
        
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                var playerManager = hit.GetComponent<PlayerManager>();
                if (playerManager == null) continue;

                switch (effectType)
                {
                    case AreaEffectType.Damage:
                        playerManager.TakeDamage(value);
                        break;
                    case AreaEffectType.Heal:
                        playerManager.Heal(value);
                        break;
                }
            }
        }
        
        yield return null;
    }
    
    public void UpdateEnemyData(EnemyData newData)
    {
        enemyData = newData;
        if (isInitialized)
        {
            originalSpeed = enemyData.moveSpeed;
        }
    }
    
    public void ForceState(string stateName)
    {
        switch (stateName.ToLower())
        {
            case "idle":
                currentState = UniversalEnemyState.Idle;
                break;
            case "patrol":
                currentState = UniversalEnemyState.Patrol;
                break;
            case "chase":
                currentState = UniversalEnemyState.Chase;
                break;
            case "attack":
                currentState = UniversalEnemyState.Attack;
                break;
            case "retreat":
                currentState = UniversalEnemyState.Retreat;
                break;
        }
    }
    
    public void AddSpecialPassive(ScriptableSpecialPassive passive)
    {
        if (activePassives == null)
            activePassives = new List<ScriptableSpecialPassive>();
            
        if (!activePassives.Contains(passive))
        {
            activePassives.Add(passive);
            if (controller != null)
            {
                passive.Activate(controller);
            }
        }
    }
    
    public void RemoveSpecialPassive(ScriptableSpecialPassive passive)
    {
        if (activePassives != null)
        {
            activePassives.Remove(passive);
        }
    }
    
    public string GetCurrentState()
    {
        return currentState.ToString();
    }
    
    public bool CanAttack()
    {
        return currentState == UniversalEnemyState.Attack || currentState == UniversalEnemyState.Chase;
    }
    
    public void SetPatrolArea(Vector2 center, float radius)
    {
        startPosition = center;
        enemyData.patrolDistance = radius;
        SetNewPatrolTarget();
    }
    
    public void SetTarget(Transform target)
    {
        player = target;
    }
    
    void OnDrawGizmosSelected()
    {
        if (enemyData == null) return;
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, enemyData.detectionRange);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemyData.attackRange);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(startPosition, enemyData.patrolDistance);
        
        if (enemyData.attackPatterns != null)
        {
            foreach (var pattern in enemyData.attackPatterns)
            {
                Gizmos.color = new Color(1f, 0.5f, 0f);
                Gizmos.DrawWireSphere(transform.position, pattern.Range);
            }
        }
    }
} 