using UnityEngine;
using UnityEngine.UI;

public class ItemFloatingText : MonoBehaviour
{
    public Camera mainCamera;             // 메인 카메라
    public RectTransform textUI;          // UI 텍스트 (Screen Space - Overlay용)
    public float yOffset = 1.0f;          // 아이템 위로 얼마나 띄울지 (월드 거리 기준)

    void Update()
    {
        if (textUI == null || mainCamera == null) return;

        // 아이템의 월드 위치 + 오프셋 → 화면 좌표로 변환
        Vector3 worldPos = transform.position + Vector3.up * yOffset;
        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);

        textUI.position = screenPos;
    }
}