/**
* 아이템 로드 및 관리
*/
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    public List<ItemData> items;

    public ItemData GetItemById(int id)
    {
        return items.Find(item => item.id == id);
    }

    public ItemData GetItemByName(string name)
    {
        return items.Find(item => item.itemName == name);
    }

    public void AddItem(ItemData item)
    {
        if (!items.Contains(item))
        {
            items.Add(item);
        }
    }

    public void RemoveItem(ItemData item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
        }
    }
}