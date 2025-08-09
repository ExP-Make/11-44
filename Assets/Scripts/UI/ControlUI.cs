using UnityEngine;
using UnityEngine.UI;

public class ControlUI : MonoBehaviour
{
    [Header("Buttons")]
    public Button leftButton;
    public Button rightButton;
    public Button interactButton;
    public Button itemButton;

    private bool isLeftPressed = false;
    private bool isRightPressed = false;
    private InventoryUI inventoryUI;

    private void Awake()
    {
        interactButton.onClick.RemoveAllListeners();
        itemButton.onClick.RemoveAllListeners();
    }

    private void Start()
    {
        inventoryUI = FindFirstObjectByType<InventoryUI>();
        if (inventoryUI == null)
        {
            Debug.LogError("InventoryUI not found in the scene!");
        }
    }

    void Update()
    {
        bool isInventoryOpen = (inventoryUI != null && inventoryUI.IsPanelActive());
        bool isDialogueOpen = DialogManager.Instance.IsDialogOpen();
        bool isUIOpen = isDialogueOpen || isInventoryOpen;

        // 아이템 버튼은 대화창이 열려있을 때만 비활성화
        itemButton.gameObject.SetActive(!isDialogueOpen);

        // 나머지 버튼들은 UI가 열려있으면 비활성화
        leftButton.gameObject.SetActive(!isUIOpen);
        rightButton.gameObject.SetActive(!isUIOpen);
        interactButton.gameObject.SetActive(!isUIOpen);

        // 버튼이 비활성화되면 입력을 중지
        if (!leftButton.gameObject.activeSelf)
        {
            isLeftPressed = false;
        }
        if (!rightButton.gameObject.activeSelf)
        {
            isRightPressed = false;
        }

        if (isLeftPressed)
        {
            InputManager.Instance.SetHorizontal(-1f);
        }
        else if (isRightPressed)
        {
            InputManager.Instance.SetHorizontal(1f);
        }
        else
        {
            InputManager.Instance.SetHorizontal(0f);
        }
    }

    // Left Button
    public void OnLeftButtonDown()
    {
        isLeftPressed = true;
    }

    public void OnLeftButtonUp()
    {
        isLeftPressed = false;
    }

    // Right Button
    public void OnRightButtonDown()
    {
        isRightPressed = true;
    }

    public void OnRightButtonUp()
    {
        isRightPressed = false;
    }

    // Interact Button
    public void OnInteractButtonDown()
    {
        InputManager.Instance.OnInteractButtonDown();
    }

    // Item Button
    public void OnItemButtonDown()
    {
        InputManager.Instance.OnItemButtonDown();
    }
}
