using UnityEngine;
using UnityEngine.UI;

public class ControlUI : MonoBehaviour
{
    public Button leftButton;
    public Button rightButton;
    //public Button jumpButton;
    public Button interactButton;
    public Button itemButton;

    private bool isLeftPressed = false;
    private bool isRightPressed = false;
    private InventoryUI inventoryUI; // 인벤토리 UI 참조

    private void Start()
    {
        inventoryUI = FindFirstObjectByType<InventoryUI>();
        if (inventoryUI == null)
        {
            Debug.LogError("InventoryUI not found in the scene!");
        }
    }

    private void Awake()
    {
        // leftButton.onClick.AddListener(() => InputManager.Instance.SetHorizontal(-1f));
        // rightButton.onClick.AddListener(() => InputManager.Instance.SetHorizontal(1f));
        //jumpButton.onClick.AddListener(InputManager.Instance.OnJumpButtonClicked);
        interactButton.onClick.AddListener(() => InputManager.Instance.OnInteractButtonDown());
        itemButton.onClick.AddListener(() => InputManager.Instance.OnItemButtonDown());
    }

    void Update()
    {
        bool isUIOpen = DialogManager.Instance.IsDialogOpen() || (inventoryUI != null && inventoryUI.IsPanelActive());

        // 아이템 버튼은 인벤토리가 열려있을 때도 보여야 하므로 따로 처리
        itemButton.gameObject.SetActive(!DialogManager.Instance.IsDialogOpen());

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
        // 수평 입력 처리
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

    public void OnLeftButtonDown()
    {
        isLeftPressed = true;
    }

    public void OnLeftButtonUp()
    {
        isLeftPressed = false;
    }

    public void OnRightButtonDown()
    {
        isRightPressed = true;
    }

    public void OnRightButtonUp()
    {
        isRightPressed = false;
    }
}