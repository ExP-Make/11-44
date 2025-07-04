using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraZoomAndClamp : MonoBehaviour
{
    public SpriteRenderer backgroundRenderer;

    private Camera cam;
    private float halfCameraWidth;
    private float minX, maxX;
    private float minY, maxY;

    void Start()
    {
        cam = Camera.main;

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

        var vcam = GetComponent<CinemachineCamera>();
        if (vcam == null)
        {
            Debug.LogError("CinemachineCamera 컴포넌트가 없습니다.");
            return;
        }

        vcam.Follow = player.transform;
        vcam.LookAt = player.transform;

        SetupCameraZoomAndBounds();
    }

    void SetupCameraZoomAndBounds()
    {
        // float bgHeight = backgroundRenderer.bounds.size.y;
        // float bgWidth = backgroundRenderer.bounds.size.x;

        // // 배경 높이에 맞게 줌 설정
        // cam.orthographicSize = bgHeight / 2f;

        // float screenAspect = (float)Screen.width / Screen.height;
        // if (screenAspect > (bgWidth / bgHeight))
        // {
        //     // 화면이 너무 넓으면 너비 기준으로 보정
        //     cam.orthographicSize = (bgWidth / screenAspect) / 2f;
        // }

        // 좌우 이동 제한 값 계산
        halfCameraWidth = cam.orthographicSize * cam.aspect;
        float bgLeft = backgroundRenderer.bounds.min.x;
        float bgRight = backgroundRenderer.bounds.max.x;
        float bgUp = backgroundRenderer.bounds.max.y;
        float bgDown = backgroundRenderer.bounds.min.y;
        minX = bgLeft + halfCameraWidth;
        maxX = bgRight - halfCameraWidth;
        minY = bgDown + cam.orthographicSize;
        maxY = bgUp - cam.orthographicSize;
    }

    void LateUpdate()
    {
        // X축 위치 제한 적용
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;
    }
}
