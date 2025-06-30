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

    public void OnClickLoad()
    {
        if (SaveSystem.SaveFileExists())
        {
            SaveData data = SaveSystem.LoadGame();
            SceneManager.LoadScene(data.currentScene);
            // 게임 씬이 로드되면 거기서 위치 및 아이템 복원 처리
        }
        else
        {
            Debug.Log("저장된 게임이 없습니다.");
        }
    }
    public void OnClickExit()
    {
        Debug.Log("게임 종료!");
        Application.Quit(); // 빌드된 상태에서만 작동
    }
}