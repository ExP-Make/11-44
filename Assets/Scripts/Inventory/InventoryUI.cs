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
    public Material silhouetteMaterial; // 실루엣 머티리얼

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
            TextMeshProUGUI nameText = slot.transform.Find("Name").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI descriptionText = slot.transform.Find("Description").GetComponent<TextMeshProUGUI>();

            iconImage.sprite = itemData.icon[0];

            var invItem = playerInventory.inventoryItems.Find(i => i.itemData.id == itemData.id);
            if (invItem != null)
            {
                iconImage.sprite = itemData.icon[0];
                nameText.text = invItem.itemData.itemName;
                descriptionText.text = invItem.itemData.description;
            }
            else
            {
                iconImage.material = silhouetteMaterial;
                nameText.text = "";
                descriptionText.text = "";
            }
        }
    }
}
