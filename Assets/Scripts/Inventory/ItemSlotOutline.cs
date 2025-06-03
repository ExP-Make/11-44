using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlotOutline : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public Outline outline;

    public void OnSelect(BaseEventData eventData)
    {
        if (outline != null)
        {
            outline.enabled = true;
        }
    }
    
    public void OnDeselect(BaseEventData eventData)
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }
}
