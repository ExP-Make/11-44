using TMPro;
using UnityEngine;

public class TypeEffect : MonoBehaviour
{
    string fullText;
    public float typingDelay = 0.1f;
    TextMeshProUGUI msgText;
    int index;
    private void Awake()
    {
        msgText = GetComponent<TextMeshProUGUI>();
    }

    public void SetMessage(string message)
    {
        fullText = message;
        EffectStart();
    }

    void EffectStart()
    {
        msgText.text = "";
        index = 0;
        InvokeRepeating(nameof(EffectUpdate), 0.1f, typingDelay);
    }

    void EffectUpdate()
    {
        if (msgText.text == fullText)
        {
            //Effect End
            return;
        }
        msgText.text += fullText[index];
        index++;
    }
}
