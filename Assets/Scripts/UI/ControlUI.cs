using UnityEngine;
using UnityEngine.UI;

public class ControlUI : MonoBehaviour
{
    [Header("Buttons")]
    public Button leftButton;
    public Button rightButton;
    public Button interactButton;
    public Button itemButton;
    public Button pauseButton;

    private bool isLeftPressed = false;
    private bool isRightPressed = false;
    private InventoryUI inventoryUI;
    private PauseUI pauseUI;

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
        
        pauseUI = FindFirstObjectByType<PauseUI>();
        if (pauseUI == null)
        {
            Debug.LogError("PauseUI not found in the scene!");
        }
        else
        {
            pauseButton.onClick.AddListener(pauseUI.PauseGame);
            pauseUI.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        bool isInventoryOpen = inventoryUI != null && inventoryUI.IsPanelActive();
        bool isDialogueOpen = DialogManager.Instance.IsDialogOpen();
        bool isUIOpen = isDialogueOpen || isInventoryOpen;

        // UI가 열려있으면 버튼 비활성화
        itemButton.gameObject.SetActive(!isUIOpen);
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
