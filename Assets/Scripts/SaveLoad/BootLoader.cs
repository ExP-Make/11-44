using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootLoader : MonoBehaviour
{
    [Tooltip("게임의 맨 첫 번째 씬 이름")]
    public string defaultScene = "Floor_1";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Instantiate(Resources.Load("Prefabs/GameManager"));
        Instantiate(Resources.Load("Prefab/Player"));
        Instantiate(Resources.Load("Prefab/SaveManager"));
        Instantiate(Resources.Load("Prefab/Canvas"));

        StartCoroutine(LoadGameScene());
    }

    private IEnumerator LoadGameScene()
    {
        yield return new WaitForSeconds(0.5f); // 간단한 로딩 연출

        if (!SaveSystem.SaveFileExists() || GameManager.Instance.ignoreSaveData)
        {
            Debug.Log("처음부터 게임을 시작합니다.");
            SceneManager.LoadScene(defaultScene);
        }
        else
        {
            SaveData data = SaveSystem.LoadGame();
            SceneManager.LoadScene(data.currentScene);
        }
    }
}
