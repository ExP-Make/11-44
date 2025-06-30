using UnityEngine;

public class PlayerManager : PersistentSingleton<PlayerManager>
{

    private SaveData cachedSaveData;

    protected override void Awake()
    {
        base.Awake();
        if (SaveSystem.SaveFileExists())
        {
            cachedSaveData = SaveSystem.LoadGame();
            transform.position = new Vector3(cachedSaveData.playerPosX, cachedSaveData.playerPosY, 0f);
        }
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
}
