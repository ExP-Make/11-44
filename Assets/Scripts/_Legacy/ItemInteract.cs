using UnityEngine;

public class ItemInteraction : MonoBehaviour, IInteractable
{
    private bool isPlayerInRange = false;

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            OnInteract();
        }
    }

    public void OnInteract()
    {
        Debug.Log("æ∆¿Ã≈€ »πµÊ!");
        InteractionUIManager.Instance.ShowText(false);
        Destroy(gameObject);
    }

    public void ShowUI(bool show)
    {
        InteractionUIManager.Instance.ShowText(show);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            ShowUI(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            ShowUI(false);
        }
    }
}