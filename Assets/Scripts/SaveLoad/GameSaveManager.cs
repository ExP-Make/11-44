using System.Collections.Generic;
using UnityEngine;

public class GameSaveManager : PersistentSingleton<GameSaveManager>
{

    protected override void Awake()
    {
        base.Awake();
        Debug.Log("GameSaveManager Awake called");
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            SaveGame();
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            SaveSystem.DeleteSave();
        }
    }

    public void SaveGame()
    {
        SaveData data = new SaveData();

        // 위치
        Vector3 pos = Player.Instance.transform.position;
        data.playerPosX = pos.x;
        data.playerPosY = pos.y;

        // 씬 이름
        data.currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        // 아이템
        Inventory inventory = Player.Instance.inventory;
        data.obtainedItemIds = new List<int>(inventory.obtainedItemIds);
        if (inventory != null)
        {
            data.savedInventoryItems = inventory.ToSavedItemList();
            Debug.Log("아이템 저장 성공");
        }

        SaveSystem.SaveGame(data);
    }
}
