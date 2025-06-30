using UnityEngine;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour
{
    public string gameSceneName = "Scene1"; // �̵��� ���� �� �̸�

    public void OnClickStart()
    {
        Debug.Log("���� ����!");
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnClickLoad()
    {
        if (SaveSystem.SaveFileExists())
        {
            SaveData data = SaveSystem.LoadGame();
            SceneManager.LoadScene(data.currentScene);
            // ���� ���� �ε�Ǹ� �ű⼭ ��ġ �� ������ ���� ó��
        }
        else
        {
            Debug.Log("����� ������ �����ϴ�.");
        }
    }
    public void OnClickExit()
    {
        Debug.Log("���� ����!");
        Application.Quit(); // ����� ���¿����� �۵�
    }
}