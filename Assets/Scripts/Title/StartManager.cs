using UnityEngine;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour
{
    public string gameSceneName = "BootScene"; // 이동할 게임 씬 이름

    public void OnClickStart()
    {
        Debug.Log("New Game");
        GameManager.Instance.ignoreSaveData = true;
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnClickLoad()
    {
        // TODO : 똑같이 BootScene으로 이동하고, BootScene에서 세이브 데이터 여부 판별
        GameManager.Instance.ignoreSaveData = false;
        SceneManager.LoadScene(gameSceneName);
    }
    public void OnClickExit()
    {
        Debug.Log("게임 종료!");
        Application.Quit(); // 빌드된 상태에서만 작동
    }
}