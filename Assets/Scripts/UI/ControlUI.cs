using UnityEngine;
using UnityEngine.UI;

public class ControlUI : MonoBehaviour
{
    [Header("Buttons")]
    public Button leftButton;
    public Button rightButton;
    public Button interactButton;
    public Button itemButton;

    [Header("Button Sprites")]
    public Sprite leftButtonNormal;
    public Sprite leftButtonPressed;
    public Sprite rightButtonNormal;
    public Sprite rightButtonPressed;
    public Sprite interactButtonNormal;
    public Sprite interactButtonPressed;
    public Sprite itemButtonNormal;
    public Sprite itemButtonPressed;

    private Image leftButtonImage;
    private Image rightButtonImage;
    private Image interactButtonImage;
    private Image itemButtonImage;

    private bool isLeftPressed = false;
    private bool isRightPressed = false;
    private InventoryUI inventoryUI;

    private void Awake()
    {
        // Get Image components from buttons
        leftButtonImage = leftButton.GetComponent<Image>();
        rightButtonImage = rightButton.GetComponent<Image>();
        interactButtonImage = interactButton.GetComponent<Image>();
        itemButtonImage = itemButton.GetComponent<Image>();

        // Remove existing listeners as we will handle them with EventTriggers
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

        // Set initial button images
        if (leftButtonImage != null) leftButtonImage.sprite = leftButtonNormal;
        if (rightButtonImage != null) rightButtonImage.sprite = rightButtonNormal;
        if (interactButtonImage != null) interactButtonImage.sprite = interactButtonNormal;
        if (itemButtonImage != null) itemButtonImage.sprite = itemButtonNormal;
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

        // 버튼이 비활성화되면 입력을 중지하고 스프라이트를 원래대로 되돌림
        if (!leftButton.gameObject.activeSelf)
        {
            isLeftPressed = false;
            if (leftButtonImage.sprite != leftButtonNormal)
                leftButtonImage.sprite = leftButtonNormal;
        }
        if (!rightButton.gameObject.activeSelf)
        {
            isRightPressed = false;
            if (rightButtonImage.sprite != rightButtonNormal)
                rightButtonImage.sprite = rightButtonNormal;
        }
        if (!interactButton.gameObject.activeSelf)
        {
            if (interactButtonImage != null && interactButtonImage.sprite != interactButtonNormal)
                interactButtonImage.sprite = interactButtonNormal;
        }
        if (!itemButton.gameObject.activeSelf)
        {
            if (itemButtonImage != null && itemButtonImage.sprite != itemButtonNormal)
                itemButtonImage.sprite = itemButtonNormal;
        }

        // 버튼이 눌리고 있는 상태에 따라 매 프레임 입력을 전달합니다.
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
            // 아무 버튼도 누르지 않으면 수평 입력을 0으로 설정하여 멈춥니다.
            InputManager.Instance.SetHorizontal(0f);
        }
    }

    // --- EventTrigger가 호출할 public 메소드들 ---

    // Left Button
    public void OnLeftButtonDown()
    {
        isLeftPressed = true;
        if (leftButtonImage != null) leftButtonImage.sprite = leftButtonPressed;
    }

    public void OnLeftButtonUp()
    {
        isLeftPressed = false;
        if (leftButtonImage != null) leftButtonImage.sprite = leftButtonNormal;
    }

    // Right Button
    public void OnRightButtonDown()
    {
        isRightPressed = true;
        if (rightButtonImage != null) rightButtonImage.sprite = rightButtonPressed;
    }

    public void OnRightButtonUp()
    {
        isRightPressed = false;
        if (rightButtonImage != null) rightButtonImage.sprite = rightButtonNormal;
    }

    // Interact Button
    public void OnInteractButtonDown()
    {
        InputManager.Instance.OnInteractButtonDown();
        if (interactButtonImage != null) interactButtonImage.sprite = interactButtonPressed;
    }

    public void OnInteractButtonUp()
    {
        if (interactButtonImage != null) interactButtonImage.sprite = interactButtonNormal;
    }

    // Item Button
    public void OnItemButtonDown()
    {
        InputManager.Instance.OnItemButtonDown();
        if (itemButtonImage != null) itemButtonImage.sprite = itemButtonPressed;
    }

    public void OnItemButtonUp()
    {
        if (itemButtonImage != null) itemButtonImage.sprite = itemButtonNormal;
    }
}
