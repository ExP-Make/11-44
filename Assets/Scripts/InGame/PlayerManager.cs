using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : PersistentSingleton<PlayerManager>
{
    [Header("Player Stats")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    public UnityEvent<float> OnHealthChanged;
    
    protected override void Awake()
    {
        base.Awake();
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);
        OnHealthChanged?.Invoke(currentHealth / maxHealth);

        if (currentHealth <= 0)
        {
            // Handle player death
            Debug.Log("Player Died!");
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        OnHealthChanged?.Invoke(currentHealth / maxHealth);
    }
}
