using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum eCameraMode
{
    Trace,
    Combat,
}

public class CameraController : MonoBehaviour
{
    public Transform target;     // 따라갈 대상 (플레이어)
    public Vector3 offset = new Vector3(0, 0, -10);
    public float smoothSpeed = 5f;
    
    private eCameraMode _cameraMode = eCameraMode.Trace;
    // 현재 컷신 중인지
    private bool _isCutscene;
    // 줌인 중인지
    private bool _isZoomIn;
    // 전투 돌입 시 고정될 박스
    private CameraSizeBox combatSizeBox;
    
    void Start()
    {
        FindPlayer(); // 시작 시 플레이어 찾기

        // 씬 변경 시 다시 찾도록 이벤트 등록
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindPlayer();
    }

    void FindPlayer()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            target = playerObj.transform;
        }
    }

    public void ChangeCutScene(CameraSizeBox targetSizeBox)
    {
        _cameraMode = eCameraMode.Combat;
        _isCutscene = true;
        _isZoomIn = true;
        combatSizeBox = targetSizeBox;
    }
    
    void FixedUpdate()
    {
        if (target == null) return;

        if (_cameraMode == eCameraMode.Trace)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
        }
        else if (_cameraMode == eCameraMode.Combat)
        {
            if (_isCutscene && combatSizeBox)
            {
                // 목표로 카메라 등속 이동
                Vector3 desiredPosition = new Vector3(combatSizeBox.BoxCenter.x,
                    combatSizeBox.BoxCenter.y, -10f);
                Vector3 moveDir = (desiredPosition - transform.position).normalized;
                transform.Translate(moveDir * (smoothSpeed * Time.deltaTime));
                
                // 거리 판정
                if (Vector3.Distance(desiredPosition, transform.position) < 0.1f)
                {
                    transform.position = desiredPosition;
                    _isCutscene = false;
                }
            }

            if (_isZoomIn && combatSizeBox)
            {
                // 카메라 줌인 / 줌아웃
                float currentCameraSize = Camera.main.orthographicSize;
                float desiredCameraSize = combatSizeBox.BoxSize.y / 2;
                float nextPos = (desiredCameraSize - currentCameraSize) /
                    math.abs(desiredCameraSize - currentCameraSize) * Time.deltaTime;
                Camera.main.orthographicSize = currentCameraSize + nextPos;
                
                // 거리 판정
                if (math.abs(desiredCameraSize - currentCameraSize) < 0.1f)
                {
                    Camera.main.orthographicSize = desiredCameraSize;
                    _isZoomIn = false;
                }
            }

            if (!_isCutscene && !_isZoomIn)
            {
                combatSizeBox = null;
            }
        }
    }
}