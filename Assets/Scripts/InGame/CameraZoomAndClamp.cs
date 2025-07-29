using Unity.Cinemachine;
using UnityEngine;

public class CameraZoomAndClamp : MonoBehaviour
{
    public SpriteRenderer backgroundRenderer;

    private CinemachineCamera vcam;
    private float halfCameraWidth;
    private float minX, maxX;
    private float minY, maxY;

    void Start()
    {
        vcam = GetComponent<CinemachineCamera>();
        if (vcam == null)
        {
            Debug.LogError("CinemachineCamera 컴포넌트가 없습니다.");
            return;
        }

        if (backgroundRenderer == null)
        {
            Debug.LogError("배경 SpriteRenderer가 지정되지 않았습니다.");
            return;
        }

        var player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player를 찾을 수 없습니다.");
            return;
        }

        vcam.Follow = player.transform;
        vcam.LookAt = player.transform;

        SetupCameraZoomAndBounds();
    }

    void SetupCameraZoomAndBounds()
    {
        float bgHeight = backgroundRenderer.bounds.size.y;
        float bgWidth = backgroundRenderer.bounds.size.x;

        // 배경 높이에 맞게 줌 설정
        vcam.Lens.OrthographicSize = (bgHeight / 2f) * 0.999f; // 0.1% 정도 작게 설정

        float screenAspect = (float)Screen.width / Screen.height;
        if (screenAspect > (bgWidth / bgHeight))
        {
            // 화면이 너무 넓으면 너비 기준으로 보정
            vcam.Lens.OrthographicSize = (bgWidth / screenAspect) / 2f * 0.999f; // 0.1% 정도 작게 설정
        }

        // 좌우 이동 제한 값 계산 (CinemachineConfiner에서 사용될 값)
        halfCameraWidth = vcam.Lens.OrthographicSize * vcam.Lens.Aspect;
        float bgLeft = backgroundRenderer.bounds.min.x;
        float bgRight = backgroundRenderer.bounds.max.x;
        float bgUp = backgroundRenderer.bounds.max.y;
        float bgDown = backgroundRenderer.bounds.min.y;
        minX = bgLeft + halfCameraWidth;
        maxX = bgRight - halfCameraWidth;
        minY = bgDown + vcam.Lens.OrthographicSize;
        maxY = bgUp - vcam.Lens.OrthographicSize;
    }
}
