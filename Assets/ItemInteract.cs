using UnityEngine;

public class ItemInteractWorldUI : MonoBehaviour
{
    public GameObject uiCanvas;   // World Space UI Canvas

    private bool isPlayerInRange = false;

    void Start()
    {
        uiCanvas.SetActive(false); // 처음엔 꺼두기
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("F 키 눌러서 아이템 획득!");
            uiCanvas.SetActive(false);
            Destroy(gameObject); // 아이템 사라짐
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            uiCanvas.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            uiCanvas.SetActive(false);
        }
    }
}