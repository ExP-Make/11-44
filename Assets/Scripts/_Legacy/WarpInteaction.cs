using UnityEngine;
using UnityEngine.SceneManagement;

public class WarpInteraction : MonoBehaviour, IInteractable
{
    public string targetSceneName;
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
        Debug.Log("¾À ÀüÈ¯: " + targetSceneName);
        InteractionUIManager.Instance.ShowText(false);
        SceneManager.LoadScene(targetSceneName);
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