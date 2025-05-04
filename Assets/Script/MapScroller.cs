using UnityEngine;

public class MapScroller : MonoBehaviour
{
    public Transform player;         // �÷��̾�
    public Transform mapRoot;        // ��ü �� ��Ʈ ������Ʈ
    public float scrollThreshold = 2f;  // Ǫ�� �ڽ��� ���� ũ�� (x ����)

    void Update()
    {
        float playerX = player.position.x;

        // ī�޶� �߽��� �׻� 0 ���� (Main Camera�� ����)
        float offsetX = playerX - 0f;

        if (offsetX > scrollThreshold)
        {
            float move = offsetX - scrollThreshold;
            mapRoot.position -= new Vector3(move, 0, 0);
            player.position -= new Vector3(move, 0, 0);
        }
        else if (offsetX < -scrollThreshold)
        {
            float move = offsetX + scrollThreshold;
            mapRoot.position -= new Vector3(move, 0, 0);
            player.position -= new Vector3(move, 0, 0);
        }
    }
}