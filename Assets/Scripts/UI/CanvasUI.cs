using UnityEngine;

public class CanvasUI : PersistentSingleton<CanvasUI>
{
    public DialogManager dialogManager;
    public InventoryUI inventoryUI;
    public ControlUI controlUI;
    protected override void Awake()
    {
        base.Awake();

        if (dialogManager == null)
        {
            dialogManager = GetComponentInChildren<DialogManager>();
            if (dialogManager == null)
            {
                Debug.LogError("Dialog Manager is not assigned in the CanvasUI script.");
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

        if (controlUI == null)
        {
            controlUI = GetComponentInChildren<ControlUI>();
            if (controlUI == null)
            {
                Debug.LogError("Control UI is not assigned in the CanvasUI script.");
            }
        }
    }
}
