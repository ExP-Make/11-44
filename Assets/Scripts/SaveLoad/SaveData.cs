using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public float playerPosX;
    public float playerPosY;
    // public float playerPosZ; // 2D 게임이니 필요없을듯
    public string currentScene;
    public List<SavedItem> savedInventoryItems = new List<SavedItem>();

    public List<int> obtainedItemIds = new List<int>(); // to hide items that already obtained
}

[System.Serializable]
public class SavedItem
{
    public int itemId;
    public int quantity;
}