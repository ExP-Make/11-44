using TMPro;
using UnityEngine;

public class TypeEffect : MonoBehaviour
{
    string fullText;
    public float typingDelay = 0.1f;
    TextMeshProUGUI msgText;
    int index;
    public bool isTypingComplete { get; private set; }

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
        isTypingComplete = false; // 타이핑 시작 시 false
        CancelInvoke(nameof(EffectUpdate)); // 이전 Invoke 중지
        InvokeRepeating(nameof(EffectUpdate), 0.1f, typingDelay);
    }

    void EffectUpdate()
    {
        if (msgText.text == fullText)
        {
            //Effect End
            CancelInvoke(nameof(EffectUpdate));
            isTypingComplete = true; // 타이핑 완료 시 true
            return;
        }
        msgText.text += fullText[index];
        index++;
    }

    public void CompleteTyping()
    {
        CancelInvoke(nameof(EffectUpdate));
        msgText.text = fullText;
        isTypingComplete = true;
    }
}
