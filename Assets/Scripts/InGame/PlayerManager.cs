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
                Debug.Log("�κ��丮 ���� ����");
            }
            else
            {
                Debug.LogWarning("�κ��丮 �Ǵ� ������ �����ͺ��̽��� null�Դϴ�");
            }
        }
    }

    private void LoadItemWithSave(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
        ItemObject[] allItemObjects = FindObjectsByType<ItemObject>(FindObjectsSortMode.None);
        foreach (var item in allItemObjects)
        {
            if (item.itemData == null) continue; // null üũ
            if (cachedSaveData.obtainedItemIds.Contains(item.itemData.id))
            {
                Destroy(item.gameObject); // �̹� ȹ���� ������ ����
                Debug.Log($"ȹ���� ������ ����: {item.itemData.itemName} (ID: {item.itemData.id})");
            }
        }

        gameObject.transform.position = new Vector3(-18, -3, 0f); // �� �̵��� ĳ���� ��ġ �ʱ�ȭ (�ӽ�)
    }
}
