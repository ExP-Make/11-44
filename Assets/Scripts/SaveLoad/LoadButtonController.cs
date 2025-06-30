using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoadButtonController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] Image buttonImg;
    [Tooltip("0: Save file exists, 1: No save file exists, 2: Button pressed")]
    public Sprite[] buttonSprites;
    private Button button;
    void Start()
    {
        button = GetComponent<Button>();
        SetButton();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (button.interactable)
            buttonImg.sprite = buttonSprites[2];
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        SetButton();
    }

    private void SetButton()
    {
        if (SaveSystem.SaveFileExists())
        {
            buttonImg.sprite = buttonSprites[0]; // save file exists
            button.interactable = true;
        }
        else
        {
            buttonImg.sprite = buttonSprites[1]; // no save file exists
            button.interactable = false;
        }
    }
}
