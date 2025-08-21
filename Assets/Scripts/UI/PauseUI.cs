using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [Header("Buttons")]
    public Button titleBtn;
    public Button settingBtn;
    public Button cancelBtn;
    
    void Start()
    {
        titleBtn.onClick.AddListener(GoToTitle);
        cancelBtn.onClick.AddListener(ResumeGame);
    }

    public void PauseGame()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    private void GoToTitle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Title");
    }
}
