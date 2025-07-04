using UnityEngine;
using System.Collections;

public interface IScriptableSpecialPassive
{
    string PassiveName { get; }
    PassiveTrigger Trigger { get; }
    
    bool ShouldActivate(UniversalEnemyController enemy);
    void Activate(UniversalEnemyController enemy);
    void Deactivate(UniversalEnemyController enemy);
    void PassiveUpdate(UniversalEnemyController enemy);
}

public abstract class ScriptableSpecialPassive : MonoBehaviour, IScriptableSpecialPassive
{
    [Header("Passive Properties")]
    public string passiveName = "Custom Passive";
    public PassiveTrigger trigger = PassiveTrigger.OnSpawn;
    
    [Header("Activation Conditions")]
    public float healthThreshold = 0.5f;
    public float distanceThreshold = 5f;
    public float intervalTime = 5f;
    
    [Header("Effects")]
    public GameObject activationEffect;
    public GameObject continuousEffect;
    public AudioClip activationSound;
    
    protected UniversalEnemyController currentEnemy;
    protected bool isActive = false;
    protected float lastActivationTime;
    protected float nextIntervalTime;
    
    public virtual string PassiveName => passiveName;
    public virtual PassiveTrigger Trigger => trigger;
    
    public virtual bool ShouldActivate(UniversalEnemyController enemy)
    {
        if (isActive && !AllowReactivation()) return false;
        
        switch (trigger)
        {
            case PassiveTrigger.OnSpawn:
                return !isActive;
            case PassiveTrigger.OnHealthLow:
                return enemy.GetHealthPercentage() <= healthThreshold;
            case PassiveTrigger.OnPlayerNear:
                return enemy.GetDistanceToPlayer() <= distanceThreshold;
            case PassiveTrigger.OnTakeDamage:
                return enemy.EnemyAI.WasRecentlyDamaged();
            case PassiveTrigger.OnInterval:
                return Time.time >= nextIntervalTime;
            default:
                return false;
        }
    }
    
    public virtual void Activate(UniversalEnemyController enemy)
    {
        currentEnemy = enemy;
        isActive = true;
        lastActivationTime = Time.time;
        
        if (trigger == PassiveTrigger.OnInterval)
        {
            nextIntervalTime = Time.time + intervalTime;
        }
        
        OnActivate();
        
        if (activationEffect != null)
        {
            Instantiate(activationEffect, enemy.transform.position, enemy.transform.rotation);
        }
        
        if (activationSound != null)
        {
            enemy.PlaySound(activationSound);
        }
        
        StartCoroutine(PassiveCoroutine());
    }
    
    public virtual void Deactivate(UniversalEnemyController enemy)
    {
        if (!isActive) return;
        
        isActive = false;
        OnDeactivate();
        
        if (continuousEffect != null)
        {
            Destroy(continuousEffect);
        }
    }
    
    public virtual void PassiveUpdate(UniversalEnemyController enemy)
    {
        if (isActive)
        {
            OnUpdate();
        }
    }
    
    protected virtual void OnActivate()
    {
        // TODO: Implement custom activation behavior
    }
    
    protected virtual void OnDeactivate()
    {
        // TODO: Implement custom deactivation behavior
    }
    
    protected virtual void OnUpdate()
    {
        // TODO: Implement continuous update behavior
    }
    
    protected virtual bool AllowReactivation()
    {
        return false;
    }
    
    protected virtual IEnumerator PassiveCoroutine()
    {
        while (isActive)
        {
            yield return StartCoroutine(PassiveEffect());
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    protected abstract IEnumerator PassiveEffect();
    
    protected virtual void ModifyEnemyStats(float healthMod, float speedMod, float damageMod)
    {
        if (currentEnemy != null)
        {
            currentEnemy.enemyData.maxHealth *= healthMod;
            currentEnemy.enemyData.moveSpeed *= speedMod;
        }
    }
    
    protected virtual void SpawnMinions(GameObject minionPrefab, int count, float radius)
    {
        if (currentEnemy == null || minionPrefab == null) return;
        
        for (int i = 0; i < count; i++)
        {
            Vector2 spawnPos = (Vector2)currentEnemy.transform.position + Random.insideUnitCircle * radius;
            Instantiate(minionPrefab, spawnPos, Quaternion.identity);
        }
    }
    
    protected virtual void CreateAreaEffect(Vector2 center, float radius, float damage)
    {
        var hits = HitboxUtility.CircleHit(center, radius, currentEnemy.PlayerLayer);
        foreach(var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                var playerManager = hit.GetComponent<PlayerManager>();
                if (playerManager != null)
                {
                    playerManager.TakeDamage(damage);
                }
            }
        }
    }
    
    protected virtual void StartRegeneration(float regenRate)
    {
        if (currentEnemy != null)
        {
            StartCoroutine(RegenerationCoroutine(regenRate));
        }
    }
    
    protected virtual IEnumerator RegenerationCoroutine(float regenRate)
    {
        while (isActive && currentEnemy != null && currentEnemy.IsAlive())
        {
            currentEnemy.Heal(regenRate * Time.deltaTime);
            yield return null;
        }
    }
    
    protected virtual void MakeInvisible(float duration)
    {
        if (currentEnemy != null)
        {
            StartCoroutine(InvisibilityCoroutine(duration));
        }
    }
    
    protected virtual IEnumerator InvisibilityCoroutine(float duration)
    {
        var renderer = currentEnemy.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            Color originalColor = renderer.color;
            Color invisibleColor = originalColor;
            invisibleColor.a = 0.3f;
            
            renderer.color = invisibleColor;
            
            yield return new WaitForSeconds(duration);
            
            renderer.color = originalColor;
        }
    }
    
    protected virtual void EnableShield(float duration, float shieldStrength)
    {
        if (currentEnemy != null)
        {
            StartCoroutine(ShieldCoroutine(duration, shieldStrength));
        }
    }
    
    protected virtual IEnumerator ShieldCoroutine(float duration, float shieldStrength)
    {
        currentEnemy.EnemyHealth.SetInvulnerable(true, duration);
        yield return new WaitForSeconds(duration);
    }
    
    protected virtual void TeleportToPlayer(float range)
    {
        if (currentEnemy == null) return;
        
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector2 direction = Random.insideUnitCircle.normalized;
            Vector2 teleportPos = (Vector2)player.transform.position + direction * range;
            
            currentEnemy.transform.position = teleportPos;
        }
    }
    
    protected virtual void TriggerEvent(string eventName)
    {
        EventBus.Broadcast(eventName);
    }
    
    void OnDrawGizmosSelected()
    {
        if (trigger == PassiveTrigger.OnPlayerNear)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, distanceThreshold);
        }
        
        DrawCustomGizmos();
    }
    
    protected virtual void DrawCustomGizmos()
    {
        // TODO: Implement custom gizmo drawing
    }
} 