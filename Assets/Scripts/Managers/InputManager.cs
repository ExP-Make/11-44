using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    public float Horizontal { get; private set; }
    public bool jumpPressed { get; private set; }
    public bool jumpReleased { get; private set; }
    private bool interactPressed_Internal;
    private bool itemPressed_Internal; // itemPressed를 내부 변수로 변경

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetHorizontal(float value) => Horizontal = value;
    public void ResetHorizontal() => Horizontal = 0;
    public void OnJumpButtonDown() => jumpPressed = true;
    public void OnJumpButtonUp()
    {
        jumpReleased = true;
        jumpPressed = false;
    }

    public void OnInteractButtonDown() => interactPressed_Internal = true;
    public void ResetInteractionButton() => interactPressed_Internal = false;
    public void OnItemButtonDown() => itemPressed_Internal = true;
    public void ResetItemButton() => itemPressed_Internal = false;
    private void Update()
    {
        // 키보드 입력 처리
        float keyboardHorizontal = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(keyboardHorizontal) > 0.1f) // 키보드 입력이 있을 경우
        {
            Horizontal = keyboardHorizontal;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpPressed = true;
        }
        if (Input.GetButtonUp("Jump"))
        {
            jumpPressed = false;
        }
        if (Input.GetButtonDown("Interact"))
        {
            interactPressed_Internal = true;
        }
    }

    private void LateUpdate()
    {
        // 매 프레임 끝에서 입력 플래그 초기화
        interactPressed_Internal = false;
        itemPressed_Internal = false;
    }

    // 외부에서 호출하여 사용. 호출 시 플래그가 초기화됨
    public bool GetInteractButtonDown()
    {
        return interactPressed_Internal;
    }

    // 외부에서 호출하여 사용. 호출 시 플래그가 초기화됨
    public bool GetItemButtonDown()
    {
        bool result = itemPressed_Internal;
        if (result)
        {
            itemPressed_Internal = false;
        }
        return result;
    }
    public void ResetInputFlags()
    {
        Horizontal = 0;
        jumpPressed = false;
        jumpReleased = false;
        interactPressed_Internal = false;
    }
}
