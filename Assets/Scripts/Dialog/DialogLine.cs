using UnityEngine;

[System.Serializable]
public class DialogLine
{
    public string speakerName;
    public Sprite portrait;
    [TextArea]
    public string message;
}
