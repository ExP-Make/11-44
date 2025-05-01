/**
* 인벤토리 시스템
* 플레이어 오브젝트에 적용하는 스크립트
*/
using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour {
    // public ItemDatabase itemDatabase; // 아이템 데이터베이스
    public List<InventoryItem> inventoryItems; // 인벤토리 아이템 리스트

    private void Start()
    {
        inventoryItems = new List<InventoryItem>();
    }

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