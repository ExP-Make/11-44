using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class UniversalEnemyHealth : MonoBehaviour
{
    [Header("Health Configuration")]
    public float maxHealth = 100f;
    public float currentHealth;
    public bool isInvulnerable = false;
    public float invulnerabilityDuration = 0.1f;
    
    [Header("Health Bar")]
    public GameObject healthBarPrefab;
    public Vector3 healthBarOffset = new Vector3(0, 1.5f, 0);
    public bool showHealthBar = true;
    public bool hideHealthBarWhenFull = true;
    
    [Header("Damage Effects")]
    public Color damageFlashColor = Color.red;
    public float damageFlashDuration = 0.1f;
    public GameObject damageNumberPrefab;
    public bool showDamageNumbers = true;
    
    [Header("Death Effects")]
    public GameObject deathEffect;
    public AudioClip deathSound;
    public float deathDelay = 2f;
    
    [Header("Drop Settings")]
    public DropItem[] dropItems;
    
    [Header("Events")]
    public UnityEvent<float> OnDamageTaken = new UnityEvent<float>();
    public UnityEvent<float, float> OnHealthChanged = new UnityEvent<float, float>();
    public UnityEvent OnDeath = new UnityEvent();
    
    private EnemyData enemyData;
    private UniversalEnemyController controller;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private GameObject healthBarInstance;
    private UniversalHealthBar healthBar;
    private bool isDead = false;
    private float lastDamageTime;
    
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }
    
    public void Initialize(EnemyData data, UniversalEnemyController enemyController)
    {
        enemyData = data;
        controller = enemyController;
        
        if (data != null)
        {
            maxHealth = data.maxHealth;
            currentHealth = maxHealth;
            
            if (data.dropItems != null && data.dropItems.Length > 0)
            {
                dropItems = data.dropItems;
            }
            
            if (data.deathEffect != null)
            {
                deathEffect = data.deathEffect;
            }
            
            if (data.deathSound != null)
            {
                deathSound = data.deathSound;
            }
            
            damageFlashColor = data.damageFlashColor;
        }

        if (showHealthBar)
        {
            CreateHealthBar();
        }
    }
    
    void Update()
    {
        if (isInvulnerable && Time.time - lastDamageTime > invulnerabilityDuration)
        {
            isInvulnerable = false;
        }
        
        UpdateHealthBar();
    }
    
    public void UpdateEnemyData(EnemyData newData)
    {
        if (newData != null)
        {
            float healthPercentage = GetHealthPercentage();
            
            maxHealth = newData.maxHealth;
            currentHealth = maxHealth * healthPercentage;
            
            enemyData = newData;
            
            if (newData.dropItems != null && newData.dropItems.Length > 0)
            {
                dropItems = newData.dropItems;
            }
            
            damageFlashColor = newData.damageFlashColor;
        }
    }
    
    void CreateHealthBar()
    {
        if (healthBarPrefab != null)
        {
            healthBarInstance = Instantiate(healthBarPrefab, transform);
            healthBarInstance.transform.localPosition = healthBarOffset;
            
            healthBar = healthBarInstance.GetComponent<UniversalHealthBar>();
            if (healthBar != null)
            {
                healthBar.Initialize(maxHealth);
            }
            
            if (hideHealthBarWhenFull)
            {
                healthBarInstance.SetActive(false);
            }
        }
    }
    
    void UpdateHealthBar()
    {
        if (healthBar == null) return;
        
        healthBar.UpdateHealth(currentHealth, maxHealth);
        
        if (hideHealthBarWhenFull)
        {
            bool shouldShow = currentHealth < maxHealth;
            healthBarInstance.SetActive(shouldShow);
        }
    }
    
    public void TakeDamage(float damage, Vector2 damageDirection = default)
    {
        if (isDead || isInvulnerable || damage <= 0) return;
        
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        lastDamageTime = Time.time;
        
        isInvulnerable = true;
        StartCoroutine(DamageFlashCoroutine());
        
        if (showDamageNumbers)
        {
            ShowDamageNumber(damage);
        }
        
        OnDamageTaken?.Invoke(damage);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        if (controller != null)
        {
            controller.PlaySoundFromData("damage");
        }
        
        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }
    
    public void Heal(float healAmount)
    {
        if (isDead) return;
        
        float oldHealth = currentHealth;
        currentHealth += healAmount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        
        if (currentHealth != oldHealth)
        {
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            
            if (showDamageNumbers)
            {
                ShowHealNumber(healAmount);
            }
        }
    }
    
    public void SetHealth(float newHealth)
    {
        currentHealth = Mathf.Clamp(newHealth, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }
    
    public void SetMaxHealth(float newMaxHealth, bool healToFull = false)
    {
        maxHealth = newMaxHealth;
        
        if (healToFull)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth = Mathf.Min(currentHealth, maxHealth);
        }
        
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
    }
    
    IEnumerator DamageFlashCoroutine()
    {
        if (spriteRenderer == null) yield break;

        float elapsed = 0f;
        while (elapsed < damageFlashDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / damageFlashDuration;
            spriteRenderer.color = Color.Lerp(originalColor, damageFlashColor, 1f - t);
            yield return null;
        }
        spriteRenderer.color = originalColor;
    }
    
    void ShowDamageNumber(float damage)
    {
        if (damageNumberPrefab != null)
        {
            GameObject damageNumber = Instantiate(damageNumberPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
            
            UniversalDamageNumber damageScript = damageNumber.GetComponent<UniversalDamageNumber>();
            if (damageScript != null)
            {
                damageScript.Initialize(damage, false);
            }
        }
    }
    
    void ShowHealNumber(float healAmount)
    {
        if (damageNumberPrefab != null)
        {
            GameObject healNumber = Instantiate(damageNumberPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
            
            UniversalDamageNumber healScript = healNumber.GetComponent<UniversalDamageNumber>();
            if (healScript != null)
            {
                healScript.Initialize(healAmount, true);
            }
        }
    }
    
    void Die()
    {
        if (isDead) return;
        
        isDead = true;
        OnDeath?.Invoke();
        
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, transform.rotation);
        }
        
        if (deathSound != null && controller != null)
        {
            controller.PlaySound(deathSound);
        }
        
        DropItems();
        
        if (healthBarInstance != null)
        {
            healthBarInstance.SetActive(false);
        }
        
        StartCoroutine(DeathCoroutine());
    }
    
    private IEnumerator DeathCoroutine()
    {
        yield return new WaitForSeconds(deathDelay);
        Destroy(gameObject);
    }
    
    void DropItems()
    {
        ItemDropUtility.DropItems(enemyData.dropItems, transform.position);
    }
    
    public float GetCurrentHealth()
    {
        return currentHealth;
    }
    
    public float GetMaxHealth()
    {
        return maxHealth;
    }
    
    public float GetHealthPercentage()
    {
        return maxHealth > 0 ? currentHealth / maxHealth : 0f;
    }
    
    public bool IsAlive()
    {
        return !isDead && currentHealth > 0;
    }
    
    public bool IsInvulnerable()
    {
        return isInvulnerable;
    }
    
    public void SetInvulnerable(bool invulnerable, float duration = 0f)
    {
        isInvulnerable = invulnerable;
        
        if (invulnerable && duration > 0f)
        {
            lastDamageTime = Time.time + duration - invulnerabilityDuration;
        }
    }
    
    public void ShowHealthBar(bool show)
    {
        showHealthBar = show;
        
        if (healthBarInstance != null)
        {
            healthBarInstance.SetActive(show);
        }
    }
    
    public void SetHealthBarOffset(Vector3 offset)
    {
        healthBarOffset = offset;
        
        if (healthBarInstance != null)
        {
            healthBarInstance.transform.localPosition = offset;
        }
    }
    
    void OnDestroy()
    {
        if (healthBarInstance != null)
        {
            Destroy(healthBarInstance);
        }
    }
}

public class UniversalHealthBar : MonoBehaviour
{
    [Header("Health Bar Components")]
    public UnityEngine.UI.Slider healthSlider;
    public UnityEngine.UI.Image fillImage;
    public UnityEngine.UI.Image backgroundImage;
    
    [Header("Colors")]
    public Color healthyColor = Color.green;
    public Color damagedColor = Color.yellow;
    public Color criticalColor = Color.red;
    
    private float maxHealth;
    private Camera mainCamera;
    
    void Start()
    {
        mainCamera = Camera.main;
        
        if (healthSlider == null)
        {
            healthSlider = GetComponent<UnityEngine.UI.Slider>();
        }
        
        if (fillImage == null && healthSlider != null)
        {
            fillImage = healthSlider.fillRect.GetComponent<UnityEngine.UI.Image>();
        }
    }
    
    void Update()
    {
        if (mainCamera != null)
        {
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                           mainCamera.transform.rotation * Vector3.up);
        }
    }
    
    public void Initialize(float maxHealthValue)
    {
        maxHealth = maxHealthValue;
        
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
        }
    }
    
    public void UpdateHealth(float currentHealth, float maxHealthValue)
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
            
            if (fillImage != null)
            {
                float healthPercentage = currentHealth / maxHealthValue;
                
                if (healthPercentage > 0.6f)
                {
                    fillImage.color = healthyColor;
                }
                else if (healthPercentage > 0.3f)
                {
                    fillImage.color = damagedColor;
                }
                else
                {
                    fillImage.color = criticalColor;
                }
            }
        }
    }
    
    public void SetMaxHealth(float newMaxHealth)
    {
        maxHealth = newMaxHealth;
        
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
        }
    }
}

public class UniversalDamageNumber : MonoBehaviour
{
    [Header("Damage Number Settings")]
    public float moveSpeed = 2f;
    public float lifeTime = 2f;
    public float fadeTime = 0.5f;
    
    [Header("Colors")]
    public Color damageColor = Color.red;
    public Color healColor = Color.green;
    
    private TMPro.TextMeshPro textMesh;
    private float spawnTime;
    private bool isHeal;
    
    void Start()
    {
        textMesh = GetComponent<TMPro.TextMeshPro>();
        if (textMesh == null)
        {
            textMesh = gameObject.AddComponent<TMPro.TextMeshPro>();
        }
        
        spawnTime = Time.time;
        Destroy(gameObject, lifeTime);
    }
    
    void Update()
    {
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;
        
        float elapsed = Time.time - spawnTime;
        
        if (elapsed > lifeTime - fadeTime)
        {
            float fadeProgress = (elapsed - (lifeTime - fadeTime)) / fadeTime;
            Color color = textMesh.color;
            color.a = 1f - fadeProgress;
            textMesh.color = color;
        }
    }
    
    public void Initialize(float value, bool heal)
    {
        isHeal = heal;
        
        if (textMesh != null)
        {
            textMesh.text = heal ? "+" + value.ToString("F0") : value.ToString("F0");
            textMesh.color = heal ? healColor : damageColor;
            textMesh.fontSize = 4f;
            textMesh.alignment = TMPro.TextAlignmentOptions.Center;
        }
    }
} 