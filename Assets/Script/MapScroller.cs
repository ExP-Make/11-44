using UnityEngine;

public class MapScroller : MonoBehaviour
{
    public Transform player;         // 플레이어
    public Transform mapRoot;        // 전체 맵 루트 오브젝트
    public float scrollThreshold = 2f;  // 푸른 박스의 절반 크기 (x 방향)

    void Update()
    {
        float playerX = player.position.x;

        // 카메라 중심은 항상 0 기준 (Main Camera는 고정)
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