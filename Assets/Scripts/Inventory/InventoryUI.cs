using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class InventoryUI : PersistentSingleton<InventoryUI>
{
    [SerializeField] GameObject inventoryPanel;
    [SerializeField] Transform itemGridParent;

    [SerializeField] Inventory playerInventory; // 참조 필요
    [SerializeField] ItemDatabase itemDatabase;

    // itemslot
    [SerializeField] GameObject itemSlotPrefab;
    [SerializeField] Material silhouetteMaterial; // 실루엣 머티리얼

    // DetailPanel
    [SerializeField] Image detailIcon;
    [SerializeField] TextMeshProUGUI detailName;
    [SerializeField] TextMeshProUGUI detailDescription;

    private Dictionary<int, GameObject> itemSlots = new Dictionary<int, GameObject>();

    protected override void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 중복 제거
            return;
        }
        base.Awake();
        if (inventoryPanel == null)
        {
            Debug.LogError("Inventory Panel is not assigned in the InventoryUI script.");
        }
        if (itemGridParent == null)
        {
            Debug.LogError("Item Grid Parent is not assigned in the InventoryUI script.");
        }
        if (playerInventory == null)
        {
            playerInventory = PlayerManager.Instance.GetComponent<Inventory>();
            if (playerInventory == null)
            {
                Debug.LogError("Player Inventory is not found on PlayerManager.");
            }
        }
        if (itemDatabase == null)
        {
            itemDatabase = Resources.Load<ItemDatabase>("ItemDatabase");
            if (itemDatabase == null)
            {
                Debug.LogError("Item Database is not found in Resources.");
            }
        }
    }

    void Update()
    {
        if (!GameManager.Instance.isDialogOpen && Input.GetKeyDown(KeyCode.E))
        {
            bool isActive = inventoryPanel.activeSelf;
            //Debug.Log("Inventory UI Toggle: " + (isActive ? "Closing" : "Opening"));
            inventoryPanel.SetActive(!isActive);

            if (!isActive)
                RefreshUI();
        }
    }

    /// <summary>
    /// 인벤토리 화면 새로고침. 인벤토리 화면을 켤 때마다 호출됨
    /// </summary>
    void RefreshUI()
    {
        // 새로고침을 위한 리셋
        foreach (Transform child in itemGridParent)
            Destroy(child.gameObject);
        ResetDetailPanel();

        // 현재 아이템 상황에 맞춰서 업데이트
        foreach (var itemData in itemDatabase.items)
        {
            //Debug.Log("Item ID: " + itemData.id + ", Name: " + itemData.itemName);
            GameObject slot = Instantiate(itemSlotPrefab, itemGridParent);
            Image iconImage = slot.transform.Find("Icon").GetComponent<Image>();

            var invItem = playerInventory.inventoryItems.Find(i => i.itemData.id == itemData.id);
            iconImage.sprite = itemData.icon[0];
            if (invItem == null)
            {
                iconImage.material = silhouetteMaterial;
            }

            Button button = slot.GetComponent<Button>();
            button.onClick.AddListener(() => UpdateDetailPanel(itemData, invItem != null));
        }
    }

    void ResetDetailPanel()
    {
        detailIcon.sprite = null;
        detailIcon.material = null;
        ChangeAlpha(detailIcon, 0f);
        detailName.text = "";
        detailDescription.text = "";
    }
    void UpdateDetailPanel(ItemData itemData, bool isObtained)
    {
        detailIcon.sprite = itemData.icon[0];
        detailIcon.material = isObtained ? null : silhouetteMaterial;
        ChangeAlpha(detailIcon, 1f);
        detailName.text = isObtained ? itemData.itemName : "???";
        detailDescription.text = isObtained ? itemData.description : "???";
    }

    void ChangeAlpha(Image image, float alpha)
    {
        Color tempColor = image.color;
        tempColor.a = alpha;
        image.color = tempColor;
    }
}
