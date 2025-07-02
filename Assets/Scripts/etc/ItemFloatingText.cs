using UnityEngine;
using UnityEngine.UI;

public class ItemFloatingText : MonoBehaviour
{
    public Camera mainCamera;             // ���� ī�޶�
    public RectTransform textUI;          // UI �ؽ�Ʈ (Screen Space - Overlay��)
    public float yOffset = 1.0f;          // ������ ���� �󸶳� ����� (���� �Ÿ� ����)

    void Update()
    {
        if (textUI == null || mainCamera == null) return;

        // �������� ���� ��ġ + ������ �� ȭ�� ��ǥ�� ��ȯ
        Vector3 worldPos = transform.position + Vector3.up * yOffset;
        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);

        textUI.position = screenPos;
    }
}