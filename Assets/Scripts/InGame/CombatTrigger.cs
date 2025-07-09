using System;
using Unity.VisualScripting;
using UnityEngine;

public class CombatTrigger : MonoBehaviour
{
    public CameraSizeBox TargetSizeBox;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>()) // 플레이어와 Trigger 시
        {
            // Combat Trigger 시작
            CombatManager.Instance.StartCombat();
            
            // 카메라 이동 시작
            CameraController camera = Camera.main.GetComponent<CameraController>();
            if (camera)
            {
                camera.ChangeCutScene(TargetSizeBox);
            }
                        
            gameObject.SetActive(false);
        }
    }
}
