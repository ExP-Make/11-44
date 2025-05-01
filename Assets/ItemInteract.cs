using UnityEngine;

public class ItemInteractWorldUI : MonoBehaviour
{
    public GameObject uiCanvas;   // World Space UI Canvas

    private bool isPlayerInRange = false;

    void Start()
    {
        uiCanvas.SetActive(false); // ó���� ���α�
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("F Ű ������ ������ ȹ��!");
            uiCanvas.SetActive(false);
            Destroy(gameObject); // ������ �����
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