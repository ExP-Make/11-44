using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : PersistentSingleton<PlayerManager>
{

    private SaveData cachedSaveData;

    protected override void Awake()
    {
        base.Awake();
        if (!GameManager.Instance.ignoreSaveData && SaveSystem.SaveFileExists())
        {
            cachedSaveData = SaveSystem.LoadGame();
            transform.position = new Vector3(cachedSaveData.playerPosX, cachedSaveData.playerPosY, 0f);
        }
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
    }
}
