using UnityEngine;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour
{
    public string gameSceneName = "Scene1"; // 이동할 게임 씬 이름

    public void OnClickStart()
    {
        Debug.Log("게임 시작!");
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnClickExit()
    {
        Debug.Log("게임 종료!");
        Application.Quit(); // 빌드된 상태에서만 작동
    }
}