using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance { get; private set; }
    public Animator dialogAnim;
    public TypeEffect dialogText;
    public TextMeshProUGUI speakerNameText;
    public Image portraitImage;

    private Queue<DialogLine> dialogQueue = new Queue<DialogLine>();
    private bool isDialogPlaying = false;

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

        if (dialogAnim == null)
        {
            Debug.LogError("Dialog Animator is not assigned in the DialogManager script.");
        }
    }
    
    void Update()
    {
        if (isDialogPlaying && Input.GetMouseButtonDown(0))
        {
            ShowNextLine();
        }
    }

    public void StartDialog(DialogSequence sequence)
    {
        dialogQueue.Clear();
        foreach (var line in sequence.lines)
        {
            dialogQueue.Enqueue(line);
        }

        dialogAnim.SetBool("isShow", true);
        isDialogPlaying = true;
        Debug.Log("Dialog started with " + dialogQueue.Count + " lines.");
        ShowNextLine();
    }

    private void ShowNextLine()
    {
        if (dialogQueue.Count == 0)
        {
            EndDialog();
            return;
        }
        Debug.Log("Showing next dialog line. Lines left: " + dialogQueue.Count);
        DialogLine line = dialogQueue.Dequeue();

        dialogText.SetMessage(line.message);
        speakerNameText.text = line.speakerName;
        speakerNameText.gameObject.SetActive(!string.IsNullOrEmpty(line.speakerName));

        if (line.portrait != null)
        {
            portraitImage.sprite = line.portrait;
            portraitImage.gameObject.SetActive(true);
        }
        else
        {
            portraitImage.gameObject.SetActive(false);
        }
    }

    private void EndDialog()
    {
        Debug.Log("Dialog ended.");
        dialogAnim.SetBool("isShow", false);
        isDialogPlaying = false;
    }

    public bool IsDialogOpen()
    {
        return isDialogPlaying;
    }
}
