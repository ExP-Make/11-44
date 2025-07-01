/**
* 인벤토리 시스템
* 플레이어 오브젝트에 적용하는 스크립트
*/
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Inventory : MonoBehaviour {
    // public ItemDatabase itemDatabase; // 아이템 데이터베이스
    public List<InventoryItem> inventoryItems = new List<InventoryItem>(); // 인벤토리 아이템 리스트
    public List<int> obtainedItemIds = new List<int>(); // 획득한 아이템 ID 리스트
    // private void Start()
    // {
    //     if (inventoryItems == null)
    //     {
    //         inventoryItems = new List<InventoryItem>();
    //     }
    // }

    // 저장용 데이터로 변환
    public List<SavedItem> ToSavedItemList()
    {
        Debug.Log("ToSavedItemList() 호출됨, 아이템 수: " + inventoryItems.Count);
        foreach (var item in inventoryItems)
        {
            Debug.Log($"저장할 아이템: {item.itemData.itemName}, 수량: {item.quantity}");
        }
        return inventoryItems.Select(i => new SavedItem
        {
            itemId = i.itemData.id,
            quantity = i.quantity
        }).ToList();
    }

    // 아이템 획득 현황 저장 (세이브&로드 시 아이템 표시 여부 결정용)
    public void RegisterObtainedItem(int itemId)
    {
        if (!obtainedItemIds.Contains(itemId))
        {
            obtainedItemIds.Add(itemId);
            Debug.Log($"획득한 아이템 등록: {itemId}");
        }
        else
        {
            Debug.Log($"이미 획득한 아이템: {itemId}");
        }
    }

    // 저장된 데이터로부터 복원
    public void LoadFromSavedItemList(List<SavedItem> savedItems, ItemDatabase itemDatabase)
    {
        inventoryItems.Clear();
        foreach (var saved in savedItems)
        {
            var itemData = itemDatabase.GetItemById(saved.itemId);
            if (itemData != null)
            {
                Debug.Log($"불러온 아이템: {itemData.itemName} x{saved.quantity}");
                inventoryItems.Add(new InventoryItem(itemData, saved.quantity));
            }
        }
    }

    /// <summary>
    /// 인벤토리에 아이템 추가
    /// </summary>
    /// <param name="itemData">아이템</param>
    /// <param name="quantity">수량 (필요없나?)</param>
    public void AddItem(ItemData itemData, int quantity = 1)
    {
        InventoryItem existingItem = inventoryItems.Find(item => item.itemData.id == itemData.id);
        if (existingItem != null)
        {
            existingItem.quantity += quantity;
        }
        else
        {
            InventoryItem newItem = new InventoryItem(itemData, quantity);
            inventoryItems.Add(newItem);
        }
        //Debug.Log("현재 인벤토리 아이템 수: " + inventoryItems.Count);
    }

    public void RemoveItem(ItemData itemData, int quantity = 1)
    {
        InventoryItem existingItem = inventoryItems.Find(item => item.itemData.id == itemData.id);
        if (existingItem != null)
        {
            existingItem.quantity -= quantity;
            if (existingItem.quantity <= 0)
            {
                inventoryItems.Remove(existingItem);
            }
        }
    }
}