using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameObject dialogPanel;
    public TextMeshProUGUI dialogText;
    public GameObject scanObject;
    public bool isDialogOpen = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowDialog(string message)
    {
        dialogText.text = message;
        dialogPanel.SetActive(true);
        isDialogOpen = true;
        //Debug.Log("Dialog shown: " + message + " " + isDialogOpen);
    }

    public void HideDialog()
    {
        dialogPanel.SetActive(false);
        isDialogOpen = false;
        //Debug.Log("Dialog hidden" + isDialogOpen);
    }
}
