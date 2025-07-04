using UnityEngine;

public static class ItemDropUtility
{
    public static void DropItems(DropItem[] dropItems, Vector3 origin)
    {
        if (dropItems == null) return;
        foreach (var dropItem in dropItems)
        {
            if (dropItem.itemPrefab == null) continue;
            if (Random.value > dropItem.dropChance) continue;

            int quantity = Random.Range(dropItem.minQuantity, dropItem.maxQuantity + 1);
            for (int i = 0; i < quantity; i++)
            {
                Vector2 offset = Random.insideUnitCircle * 2f;
                Object.Instantiate(dropItem.itemPrefab, origin + (Vector3)offset, Quaternion.identity);
            }
        }
    }
} 