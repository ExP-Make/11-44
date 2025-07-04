using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossUI : MonoBehaviour
{
    [Header("UI References")]
    public Image healthFill;
    public TextMeshProUGUI nameText;

    private UniversalEnemyController bossController;

    public void Initialize(UniversalEnemyController controller)
    {
        this.bossController = controller;
        
        if (bossController == null) 
        {
            gameObject.SetActive(false);
            return;
        }

        bossController.EnemyHealth.OnHealthChanged.AddListener(UpdateHealthUI);
        
        if (nameText != null)
        {
            nameText.text = bossController.enemyData.enemyName;
        }
        
        // Set initial health
        UpdateHealthUI(bossController.GetCurrentHealth(), bossController.GetMaxHealth());
        
        gameObject.SetActive(true);
    }
    
    private void UpdateHealthUI(float currentHealth, float maxHealth)
    {
        if (healthFill != null)
        {
            healthFill.fillAmount = (maxHealth > 0) ? (currentHealth / maxHealth) : 0;
        }
    }

    private void OnDestroy()
    {
        if (bossController != null && bossController.EnemyHealth != null)
        {
            bossController.EnemyHealth.OnHealthChanged.RemoveListener(UpdateHealthUI);
        }
    }
} 