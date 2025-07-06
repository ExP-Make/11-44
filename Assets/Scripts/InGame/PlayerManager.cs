using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class PlayerManager : PersistentSingleton<PlayerManager>
{
    [Header("Player Stats")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    private SaveData cachedSaveData;
    public UnityEvent<float> OnHealthChanged;

    protected override void Awake()
    {
        base.Awake();

        currentHealth = maxHealth;

        if (!GameManager.Instance.ignoreSaveData && SaveSystem.SaveFileExists())
        {
            cachedSaveData = SaveSystem.LoadGame();
            transform.position = new Vector3(cachedSaveData.playerPosX, cachedSaveData.playerPosY, 0f);
        }
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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += LoadItemWithSave;
    }
    
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= LoadItemWithSave;
    }

    private void Start()
    {
        if (cachedSaveData != null)
        {
            Inventory inventory = GetComponent<Inventory>();
            ItemDatabase itemDatabase = Resources.Load<ItemDatabase>("Item/ItemData/ItemDatabase");
            if (inventory != null && itemDatabase != null)
            {
                inventory.LoadFromSavedItemList(cachedSaveData.savedInventoryItems, itemDatabase);
                Debug.Log("인벤토리 복원 성공");
            }
            else
            {
                Debug.LogWarning("인벤토리 또는 아이템 데이터베이스가 null입니다");
            }
        }
    }

    private void LoadItemWithSave(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
        ItemObject[] allItemObjects = FindObjectsByType<ItemObject>(FindObjectsSortMode.None);
        foreach (var item in allItemObjects)
        {
            if (item.itemData == null) continue; // null 체크
            if (cachedSaveData.obtainedItemIds.Contains(item.itemData.id))
            {
                Destroy(item.gameObject); // 이미 획득한 아이템 제거
                Debug.Log($"획득한 아이템 제거: {item.itemData.itemName} (ID: {item.itemData.id})");
            }
        }

        gameObject.transform.position = new Vector3(-18, -3, 0f); // 층 이동시 캐릭터 위치 초기화 (임시)
    }
}
