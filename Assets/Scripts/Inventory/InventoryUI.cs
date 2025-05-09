using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryPanel;
    public Transform itemGridParent;
    public GameObject itemSlotPrefab;

    public Inventory playerInventory; // 참조 필요
    public ItemDatabase itemDatabase;

    private Dictionary<int, GameObject> itemSlots = new Dictionary<int, GameObject>();
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            bool isActive = inventoryPanel.activeSelf;
            Debug.Log("Inventory UI Toggle: " + (isActive ? "Closing" : "Opening"));
            inventoryPanel.SetActive(!isActive);

            if (!isActive)
                RefreshUI();
        }
    }

    void RefreshUI()
    {
        foreach (Transform child in itemGridParent)
            Destroy(child.gameObject);

        foreach (var itemData in itemDatabase.items)
        {
            Debug.Log("Item ID: " + itemData.id + ", Name: " + itemData.itemName);
            GameObject slot = Instantiate(itemSlotPrefab, itemGridParent);
            Image iconImage = slot.transform.Find("Icon").GetComponent<Image>();
            TextMeshProUGUI quantityText = slot.transform.Find("Quantity").GetComponent<TextMeshProUGUI>();

            iconImage.sprite = itemData.icon;

            var invItem = playerInventory.inventoryItems.Find(i => i.itemData.id == itemData.id);
            if (invItem != null)
            {
                iconImage.color = new Color(1f, 1f, 1f, 1f); // 선명
                quantityText.text = invItem.quantity.ToString() + "개 보유 중";
            }
            else
            {
                iconImage.color = new Color(1f, 1f, 1f, 0.3f); // 흐릿하게
                quantityText.text = "미보유";
            }
        }
    }
}
