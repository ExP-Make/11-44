using UnityEngine;

[CreateAssetMenu(fileName = "NewDialog", menuName = "Dialog/DialogSequence")]
public class DialogSequence : ScriptableObject
{
    public DialogLine[] lines;
}