using UnityEngine;

public class InteractionUIManager : MonoBehaviour
{
    public static InteractionUIManager Instance;

    public GameObject interactionTextUI;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void ShowText(bool show)
    {
        if (interactionTextUI != null)
            interactionTextUI.SetActive(show);
    }
}