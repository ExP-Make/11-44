using UnityEngine;

public class CanvasUI : PersistentSingleton<CanvasUI>
{
    public DialogUI dialogUI;
    public InventoryUI inventoryUI;
    protected override void Awake()
    {
        base.Awake();

        if (dialogUI == null)
        {
            dialogUI = GetComponentInChildren<DialogUI>();
            if (dialogUI == null)
            {
                Debug.LogError("Dialog UI is not assigned in the CanvasUI script.");
            }
        }

        if (inventoryUI == null)
        {
            inventoryUI = GetComponentInChildren<InventoryUI>();
            if (inventoryUI == null)
            {
                Debug.LogError("Inventory UI is not assigned in the CanvasUI script.");
            }
        }
    }
}
