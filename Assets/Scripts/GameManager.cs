using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : PersistentSingleton<GameManager>
{
    public GameObject scanObject;
    public bool isDialogOpen = false;
    public bool ignoreSaveData = false;

    //Moved to DialogUI.cs

    // public GameObject dialogPanel;
    // public TextMeshProUGUI dialogText;

    protected override void Awake()
    {
        base.Awake();
        Debug.Log("GameManager Awake called");
    }
    // public void ShowDialog(string message)
    // {
    //     dialogText.text = message;
    //     dialogPanel.SetActive(true);
    //     isDialogOpen = true;
    //     //Debug.Log("Dialog shown: " + message + " " + isDialogOpen);
    // }

    // public void HideDialog()
    // {
    //     dialogPanel.SetActive(false);
    //     isDialogOpen = false;
    //     //Debug.Log("Dialog hidden" + isDialogOpen);
    // }
}
