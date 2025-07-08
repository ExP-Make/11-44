using UnityEngine;

public class NPCController : MonoBehaviour
{
    public DialogSequence dialogSequence;
    private bool isPlayerInRange = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            if (!DialogManager.Instance.IsDialogOpen())
            {
                DialogManager.Instance.StartDialog(dialogSequence);
            }
        }
    }
}
