using TMPro;
using UnityEngine;


// 현재 적용된 오브젝트 없음
public class DialogUI : PersistentSingleton<DialogUI>
{
    public GameObject dialogPanel;
    public TextMeshProUGUI dialogText;

    public void ShowDialog(string message)
    {
        dialogText.text = message;
        dialogPanel.SetActive(true);
        GameManager.Instance.isDialogOpen = true;
        //Debug.Log("Dialog shown: " + message + " " + isDialogOpen);
    }

    public void HideDialog()
    {
        dialogPanel.SetActive(false);
        GameManager.Instance.isDialogOpen = false;
        //Debug.Log("Dialog hidden" + isDialogOpen);
    }
}
